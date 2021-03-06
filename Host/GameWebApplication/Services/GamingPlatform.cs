﻿using GameWebApplication.Abstractions;
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
        private readonly List<IUserDto> _waitList = new List<IUserDto>();
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

        public async Task ChangeUserFigure(string login, Figure figure)
        {
            await Task.Run(async () =>
            {
                (await _userStorage.GetUser(login)).ChangeCurrentFigure(figure);
            });
        }

        public async Task<bool> CheckIfInQueue(string login)
        {
            return (await _userStorage.GetUser(login)).IsInQueue;
        }

        public async Task<bool> CheckIfInRound(string login)
        {
            return await Task.Run(async () =>
            {
                return (await _userStorage.GetUser(login)).IsInRound(); 
            });
        }

        public async Task<bool> ConfirmUserConnection(string login)
        {
            var user = await _userStorage.GetUser(login);
            if (user.IsActive())
            {
                user.Connect();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<bool> ConnectToPrivateSessionAsync(string login, string gameKey)
        {
            return Task.Run(async () =>
             {
                 var session = _privateSessions.Find(ps => ps.GameKey == gameKey);
                 if (session != default(PrivateSession))
                 {
                     session.Player2 = login;
                     _matchmaker.StartRegularSesionAsync(await _userStorage.GetUser(login),
                         await _userStorage.GetUser(session.Player1));
                     return true;
                 }
                 return false;
             });
        }

        public async Task<string> ConnectUserAsync(string login, string password)
        {
            var result =  await _userStorage.TryAuthorizeUser(login, password);
            if (result == "ok") (await _userStorage.GetUser(login)).Activate();
            return result;
        }

        public async Task DisconnectUserAsync(string login)
        {
            await Task.Run(async () =>
            {
                if ((await _userStorage.GetUser(login)).IsInQueue)
                {
                    await StopSearch(login);
                }
            (await _userStorage.GetUser(login)).Disactivate();
                _logger.LogWarning($"user {login} disconnected from platform");
            });
        }

        public async Task<Statistics[]> GetGlobalStatistics()
        {
            return await _userStorage.GetGlobalStatistics();
        }

        public Task<string> GetLastRoundResult(string login)
        {
            return Task.Run(async () =>
            {
                return (await _userStorage.GetUser(login)).LastRoundResult;
            });
        }

        public async Task<Statistics> GetUserStatistics(string login)
        {
            return (await _userStorage.GetUser(login)).Account.Statistics;
        }

        public async Task<bool> IsInGame(string login)
        {
            return await Task.FromResult((await _userStorage.GetUser(login)).IsInGame());
        }

        public async Task QuitCurrentGame(string login)
        {
            await Task.Run(async () =>
            {
                (await _userStorage.GetUser(login)).ExitGame();
            });
        }

        public async Task<bool> RegisterUserAsync(string login, string password)
        {
            return await _userStorage.AddUser(login, password);
        }

        public async Task StartAISessionAsync(string login)
        {
            _matchmaker.StartAISesionAsync(await _userStorage.GetUser(login));
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
                    _waitList.Add(user);
                    user.IsInQueue = true;
                    _logger.LogWarning($"user {user.Account.Login} entered Queue!");
                }
            });
        }

        public Task StopSearch(string login)
        {
            return Task.Run(() =>
            {
                lock (_queueLockObj)
                {
                    _waitList.Remove(_waitList.Find(u => u.Account.Login == login));
                }
                _logger.LogWarning($"user {login} exited search queue!");
            });
        }

        private Task FindOponentService()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if(_waitList.Count >= 2)
                    {
                        IUserDto user1;
                        IUserDto user2;
                        lock (_queueLockObj)
                        {
                            user1 = _waitList[0];
                            user2 = _waitList[1];
                            if (!user1.IsActive())
                            {
                                _waitList.Remove(user1);
                            }
                            else if (!user2.IsActive())
                            {
                                _waitList.Remove(user2);
                            }
                            else
                            {
                                _waitList.RemoveAt(0);
                                _waitList.RemoveAt(0);                                
                            }
                        }
                        user1.IsInQueue = false;
                        user2.IsInQueue = false;
                        _matchmaker.StartRegularSesionAsync(user1, user2);
                        _logger.LogWarning($"Users {user1.Account.Login} " +
                            $"and {user2.Account.Login} started game session!");
                    }
                    await Task.Delay(3000);
                }
            });
        }
    }
}
