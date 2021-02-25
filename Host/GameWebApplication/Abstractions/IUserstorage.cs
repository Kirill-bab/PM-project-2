using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    interface IUserStorage
    {// List<UserDto>
        public Task ActivateUser(IUserDto user);

        public Task AddUserSession(IUserDto user);
        public Task AddUserSession(IUserDto user1, IUserDto user2);
        public Task DisactivateUser(IUserDto user);
        public Task BanUser(IUserDto user);
        public Task UnBanUser(IUserDto user);
        public string GetUsers();
        public void InitializeUserList(string json);
}
