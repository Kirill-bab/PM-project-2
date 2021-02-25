using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace GameWebApplication.Services
{
    public class UserStorage : IUserStorage
    {
        private readonly object _usersLockObj = new object();
        private readonly object _bannedUsersLockObj = new object();
        private List<IUserDto> _users;
        private readonly List<string> _bannedUsers = new List<string>(0);

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
            _users = JsonSerializer.Deserialize<List<IUserDto>>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
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
    }
}
