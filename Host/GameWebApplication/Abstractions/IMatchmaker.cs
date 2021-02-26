using GameWebApplication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IMatchmaker
    { 
        public void StartRegularSesionAsync(IUserDto user1, IUserDto user2);
        public void StartAISesionAsync(IUserDto user);
    }
}
