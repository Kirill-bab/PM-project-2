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
            lock (_bannedUsersLockObj) 
            {
                new Timer(callback => { UnBanUser(user); }, null, 120_000, Timeout.Infinite);
                return Task.Run(() => _bannedUsers.Add(user.Account.Login));
            }
        }

        public Task<bool> CheckIfUserBanned(IUserDto user)
        {
            return Task.FromResult<bool>(_bannedUsers.Contains(user.Account.Login));
        }
        public Task DisactivateUser(IUserDto user)
        {
            lock (_usersLockObj)
            {
                return Task.Run(() =>
                _users.Find(u => u.Account.Login == user.Account.Login).Disactivate());
            }
        }

        public List<IUserDto> GetUsers()
        {
            return _users;
        }

        public void InitializeUserList(string json)
        {
            var usersAccounts = JsonSerializer.Deserialize<List<UserAccount>>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            foreach (var user in usersAccounts)
            {
                _users.Add(new UserDto(user));
            }
        }

        public Task UnBanUser(IUserDto user)
        {
            return Task.Run(() => _bannedUsers.Remove(user.Account.Login));
        }

        Task IUserStorage.ActivateUser(IUserDto user)
        {
            return Task.Run(() => _users
            .Find(u => u.Account.Login.Equals(user.Account.Login)).Activate());
        }

        public Task<bool> TryAuthorizeUser(string login, string password)
        {
            return Task.FromResult<bool>(_users.Find(u => u.Account.Login == login &&
            u.Account.Password == password) != default(IUserDto));
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
                return true;
            });
        }
    }
}
