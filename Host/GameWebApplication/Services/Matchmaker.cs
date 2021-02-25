using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class Matchmaker : IMatchmaker
    {
        private IGamePerformer _gamePerformer; //DI
        private ILogger<Matchmaker> _logger; // DI loggerFactory

        public Matchmaker(IGamePerformer gamePerformer, ILoggerFactory loggerFactory)
        {
            _gamePerformer = gamePerformer;
            _logger = loggerFactory.CreateLogger<Matchmaker>();
        }
        public async Task StartAISesionAsync(IUserDto user, CancellationToken ct)
        {
            //I used Task.Run instead of TaskFactory.StartNew to reuse threads in thread pool 
             await Task.Run(async () =>
            {
                bool timeoutHasCome = false;
                var timer = new Timer(Callback => { timeoutHasCome = true; }, null, 300_000,
                    Timeout.Infinite);
                var session = new Session(user.Account.Login, "computer");
                while (!ct.IsCancellationRequested)
                { 
                    try
                    {
                        while (!user.IsReadyForNextRound())
                        {
                            if (timeoutHasCome)
                            {
                                if (session.Rounds.Count == 0) return;
                                session.EndingReason = "session was stopped due to timeout";
                                user.RegisterNewSession(session);
                                return;
                            }
                        }

                        var round = await _gamePerformer.StartRoundWithAIAsync(user, ct,
                            new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);

                        if (round != null) session.Rounds.Add(round);
                    }
                    catch (TaskCanceledException)
                    {
                        if (session.Rounds.Count == 0) return;
                        session.EndingReason = "user quited session";
                        user.RegisterNewSession(session);
                        return;
                    }
                 }
            });
        }


        public async Task StartRegularSesionAsync(IUserDto user1, IUserDto user2,
            CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                bool timeoutHasCome = false;
                var timer = new Timer(Callback => { timeoutHasCome = true; }, null, 300_000,
                    Timeout.Infinite);
                var session = new Session(user1.Account.Login, user2.Account.Login);
                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        while (!user1.IsReadyForNextRound() && !user2.IsReadyForNextRound())
                        {
                            if (timeoutHasCome)
                            {
                                if (session.Rounds.Count == 0) return;
                                session.EndingReason = "session was stopped due to timeout";
                                user1.RegisterNewSession(session);
                                user2.RegisterNewSession(session);
                                return;
                            }
                        }

                        var round = await _gamePerformer.StartRoundWithPlayerAsync(user1, user2,
                            ct, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);

                        if (round != null) session.Rounds.Add(round);
                    }
                    catch (TaskCanceledException)
                    {
                        if (session.Rounds.Count == 0) return;
                        session.EndingReason = "user quited session";
                        user1.RegisterNewSession(session);
                        user2.RegisterNewSession(session);
                        return;
                    }
                }
            });
        }
    }
}
