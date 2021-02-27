using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace GameWebApplication.Services
{
    public class UserStorage : IUserStorage
    {
        private readonly object _usersLockObj = new object();
        private readonly object _bannedUsersLockObj = new object();
        private List<IUserDto> _users = new List<IUserDto>();
        private readonly List<string> _bannedUsers = new List<string>(0);
        private readonly ILogger<UserStorage> _logger;

        public UserStorage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UserStorage>();
        }

        public Task BanUser(IUserDto user)
        {
            new Timer(callback => { UnBanUser(user); }, null, 120_000, Timeout.Infinite);
            return Task.Run(() =>
            {
                lock (_bannedUsersLockObj)
                {
                    _bannedUsers.Add(user.Account.Login);
                }
                _logger.LogInformation($"user {user.Account.Login} banned for 3 minutes!");
            });
        }

        public Task<bool> CheckIfUserBanned(IUserDto user)
        {
            return Task.FromResult<bool>(_bannedUsers.Contains(user.Account.Login));
        }
        public Task DisactivateUser(IUserDto user)
        {
            return Task.Run(() =>
            {
                IUserDto us;
                lock (_usersLockObj)
                {
                    us = _users.Find(u => u.Account.Login == user.Account.Login);
                }
                us.Disactivate();
                _logger.LogInformation($"user {us.Account.Login} disactivated!");
            });
            
        }

        public List<IUserDto> GetUsers()
        {
            return _users;
        }

        public void InitializeUserList(string json)
        {
            _logger.LogInformation("INITIALIZATION STARTED!");
            var usersAccounts = JsonSerializer.Deserialize<List<UserAccount>>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            foreach (var user in usersAccounts)
            {
                _users.Add(new UserDto(user));
                _logger.LogInformation($"user {user.Login} initialized!");
            }
            _logger.LogInformation("INITIALIZATION ENDED!");
        }
        
        public Task UnBanUser(IUserDto user)
        {
            return Task.Run(() =>
            {
                lock (_bannedUsersLockObj)
                {
                    _bannedUsers.Remove(user.Account.Login);
                }
                _logger.LogInformation($"user {user.Account.Login} is unbanned!");
            });
        }

        public Task<string> TryAuthorizeUser(string login, string password)
        {
            return Task.Run<string>(() =>
            {
                string answer = "";
                var match = _users.Find(u => u.Account.Login == login &&
                 u.Account.Password == password);
                if ( match != default(IUserDto))
                {
                    _logger.LogInformation($"user {login} found on userList!");
                    lock (_bannedUsersLockObj)
                    {
                        if (_bannedUsers.Where(usr => usr == login).Count() != 0)
                        {
                            answer = "banned";
                            _logger.LogInformation($"user {login} found on banned List!");
                        }
                        else if (match.IsActive()) answer = "alreadyOnPlatform";
                        else answer = "ok";
                    }
                    return answer;
                }
                return "notFound";
            });
        }

        public Task<IUserDto> GetUser(string login)
        {
            return Task.FromResult(_users.Find(u => u.Account.Login == login));
        }

        public Task<bool> AddUser(string login, string password)
        {
            return Task.Run(() =>
            {
                var isAlreadyExists = _users.Find(u => u.Account.Login == login) != default(IUserDto);
                if (isAlreadyExists) return false;

                lock (_usersLockObj)
                {
                    _users.Add(new UserDto(new UserAccount(login, password)));
                }
                _logger.LogInformation($"user {login} registered!");
                return true;
            });
        }

        public Task<IStatistics[]> GetGlobalStatistics()
        {
            return Task.Run(() =>
            {
                IStatistics[] globalStats;
                lock (_usersLockObj)
                {
                    globalStats = _users
                    .Where(u => u.Account.Statistics.SessionsList.Count > 10)
                    .Select(u => u.Account.Statistics).ToArray();
                }
                return globalStats;
            });      
        }
    }
}
