using GameWebApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IGamingPlatform
    { // + userStorage + matchmaker + logger
        //+ RandomSessionQueue + PrivateSessionsList
        public Task<string> ConnectUserAsync(string login, string password);
        public Task<bool> IsInGame(string login);
        public Task<bool> RegisterUserAsync(string login, string password);
        public Task DisconnectUserAsync(string login);
        public Task StopSearch(string login);
        public Task BanUser(string login);
        public Task StartRandomSessionAsync(string login);
        public Task<string> StartPrivateSessionAsync(string login);
        public Task<bool> ConnectToPrivateSessionAsync(string login, string gameKey);
        public Task StartAISessionAsync(string login);
        public void InitializeUserStorage();
        public void SaveStorage();
        public Task<IStatistics> GetUserStatistics(string login);
        public Task<IStatistics[]> GetGlobalStatistics();
        public Task ConfirmUserConnection(string login);
    }
}
