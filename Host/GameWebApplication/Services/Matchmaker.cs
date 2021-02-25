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
                var session = new Session(user.Account.Login, "computer");
                try
                {
                    while (true)
                    {
                      

                        //await
                    }
                }
                catch(TaskCanceledException)
                {

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
