using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Library;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class GamingPlatform : IGamingPlatform
    {
        private readonly ILogger<GamingPlatform> _logger;
        private readonly IMatchmaker _matchmaker;
        private readonly IUserStorage _userStorage;
        private readonly Queue<IUserDto> _waitList;
        private readonly object _queueLockObj = new object();
        private readonly List<PrivateSession> _privateSessions = new List<PrivateSession>(0);
        private readonly object _privateLockObj = new object();
        public GamingPlatform(ILoggerFactory loggerFactory, IMatchmaker matchmaker, IUserStorage userStorage)
        {
            _logger = loggerFactory.CreateLogger<GamingPlatform>();
            _matchmaker = matchmaker ?? throw new ArgumentNullException(nameof(matchmaker));
            _userStorage = userStorage;
            FindOponentService();
        }

        public async Task BanUser(string login)
        {
           await _userStorage.BanUser( await _userStorage.GetUser(login));
        }

        public Task<bool> ConnectToPrivateSessionAsync(string login, string gameKey)
        {
            return Task.Run(async () =>
             {
                 var session = _privateSessions.Find(ps => ps.GameKey == gameKey);
                 if (session != default(PrivateSession))
                 {
                     session.Player2 = login;
                     await _matchmaker.StartRegularSesionAsync(await _userStorage.GetUser(login),
                         await _userStorage.GetUser(session.Player1));
                     return true;
                 }
                 return false;
             });
        }

        public async Task<bool> ConnectUserAsync(string login, string password)
        {
            var isRealUser =  await _userStorage.TryAuthorizeUser(login, password);
            if (!isRealUser) return false;
            (await _userStorage.GetUser(login)).Activate();
            return true;
        }

        public async Task DisconnectUserAsync(string login)
        {
            (await _userStorage.GetUser(login)).Disactivate();
            await StopSearch(login);
        }

        public async Task<IStatistics> GetUserStatistics(string login)
        {
            return (await _userStorage.GetUser(login)).Account.Statistics;
        }

        public void InitializeUserStorage()
        {
            _userStorage.InitializeUserList(FileWorker.Read("users.json"));
        }

        public async Task<bool> RegisterUserAsync(string login, string password)
        {
            return await _userStorage.AddUser(login, password);
        }

        public void SaveStorage()
        {
            FileWorker.Write("users.json", JsonSerializer
                .Serialize<List<UserAccount>>(_userStorage
                .GetUsers().Select(u => u.Account).ToList()));
        }

        public async Task StartAISessionAsync(string login)
        {
            await _matchmaker.StartAISesionAsync(await _userStorage.GetUser(login));
        }

        public Task<string> StartPrivateSessionAsync(string login)
        {
            return Task.Run(() =>
            {
                var privateSession = new PrivateSession(login, "waiting...");
                lock (_privateLockObj)
                {
                    _privateSessions.Add(privateSession);
                }
                return privateSession.GameKey;
            });
        }

        public Task StartRandomSessionAsync(string login)
        {
            return Task.Run(async () =>
            {
                var user = await _userStorage.GetUser(login);
                lock (_queueLockObj)
                {
                    _waitList.Enqueue(user);
                }
            });
        }

        public Task StopSearch(string login)
        {
            return Task.Run(() =>
            {
                var stack = new Stack<IUserDto>();
                lock (_queueLockObj)
                {
                    while(_waitList.Count > 0 || _waitList.Peek().Account.Login != login)
                    {
                        stack.Push(_waitList.Dequeue());
                    }
                    if(_waitList.TryPeek(out _))
                    {
                        _waitList.Dequeue();
                        while(stack.Count > 0)
                        {
                            _waitList.Enqueue(stack.Pop());
                        }
                    }
                }
            });
        }

        private Task FindOponentService()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    if(_waitList.Count >= 2)
                    {
                        IUserDto user1;
                        IUserDto user2;
                        lock (_queueLockObj)
                        {
                            user1 = _waitList.Dequeue();
                            user2 = _waitList.Dequeue();      
                        }
                        _matchmaker.StartRegularSesionAsync(user1,user2);
                    }
                }
            });
        }
    }
}
