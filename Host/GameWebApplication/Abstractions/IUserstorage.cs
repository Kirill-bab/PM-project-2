using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IUserStorage
    {
        public Task ActivateUser(IUserDto user);
        public Task DisactivateUser(IUserDto user);
        public Task BanUser(IUserDto user);
        public Task UnBanUser(IUserDto user);
        public List<IUserDto> GetUsers();
        public void InitializeUserList(string json);
        public Task<bool> CheckIfUserBanned(IUserDto user); 
    }
}
