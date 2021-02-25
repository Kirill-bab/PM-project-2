using GameWebApplication.Models;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    interface IGamingPlatform
    { // + userStorage + matchmaker + logger
        //+ RandomSessionQueue + PrivateSessionsList
        public Task ConnectUserAsync(IUserDto user);
        public Task DisconnectUserAsync(IUserDto user);
        public Task StartRandomSessionAsync(IUserDto user1);
        public Task StartPrivateSessionAsync(IUserDto user);
        public Task StartAISessionAsync(IUserDto user);

        public Task GetUserStatistics(IUserDto user);
    }
}
