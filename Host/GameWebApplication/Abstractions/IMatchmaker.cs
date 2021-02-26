using GameWebApplication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IMatchmaker
    { 
        public Task StartRegularSesionAsync(IUserDto user1, IUserDto user2);
        public Task StartAISesionAsync(IUserDto user);
    }
}
