using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class Matchmaker : IMatchmaker
    {
        private static readonly TaskFactory _factory = new TaskFactory();
        public async Task StartAISesionAsync(IUserDto user, CancellationToken ct,
            CancellationToken timeoutCt)
        {
             await _factory.StartNew(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var session = new Session(user.Account.Login, "computer");
                    try
                    {

                    }
                    catch (TaskCanceledException)
                    {
                        if (session.Rounds.Count == 0) return;                        
                    }
                }
            });
        }

        public async Task StartRegularSesionAsync(IUserDto user1, IUserDto user2,
            CancellationToken ct, CancellationToken timeoutCt)
        {
            throw new NotImplementedException();
        }
    }
}
