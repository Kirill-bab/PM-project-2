using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IUserStorage
    {
        public Task DisactivateUser(IUserDto user);
        public Task BanUser(IUserDto user);
        public Task UnBanUser(string login);
        public List<IUserDto> GetUsers();
        public void InitializeUserList(string json);
        public Task<bool> CheckIfUserBanned(IUserDto user);
        public Task<Statistics[]> GetGlobalStatistics(); 
        public Task<string> TryAuthorizeUser(string login, string password);
        public Task<IUserDto> GetUser(string login);
        public Task<bool> AddUser(string login, string password);
    }
}
