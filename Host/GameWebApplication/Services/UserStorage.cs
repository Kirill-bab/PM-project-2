using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class UserStorage : IUserStorage
    {
        private List<IUserDto> _users;
        private List<string> _bannsedUsers = new List<string>(0);
        public void ActivateUser(IUserDto user)
        {
            _users.Find(u => u.Account.Login.Equals(user.Account.Login)).Activate();
        }

        public Task AddUserSession(IUserDto user)
        {
            throw new NotImplementedException();
        }

        public Task AddUserSession(IUserDto user1, IUserDto user2)
        {
            throw new NotImplementedException();
        }

        public Task BanUser(IUserDto user)
        {
             
        }

        public Task DisactivateUser(IUserDto user)
        {
            throw new NotImplementedException();
        }

        public string GetUsers()
        {
            throw new NotImplementedException();
        }

        public void InitializeUserList(string json)
        {
            throw new NotImplementedException();
        }

        public Task UnBanUser(IUserDto user)
        {
            throw new NotImplementedException();
        }

        Task IUserStorage.ActivateUser(IUserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
