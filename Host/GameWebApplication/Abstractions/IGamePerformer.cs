using GameWebApplication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Abstractions
{
    public interface IGamePerformer
    {
        public Task<Round> StartRoundWithPlayerAsync(IUserDto user1, IUserDto user2,
            CancellationToken user1Ct, CancellationToken user2Ct, CancellationToken timeoutCt);
        public Task<Round> StartRoundWithAIAsync(IUserDto user, CancellationToken ct,
            CancellationToken timeoutCt);
    }
}
