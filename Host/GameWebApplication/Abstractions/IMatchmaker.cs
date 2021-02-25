using GameWebApplication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    interface IMatchmaker
    { // + logger
        public Task StartRegularSesionAsync(IUserDto user1, IUserDto user2, 
            CancellationToken ct);
        public Task StartAISesionAsync(IUserDto user, CancellationToken ct);
    }
}
