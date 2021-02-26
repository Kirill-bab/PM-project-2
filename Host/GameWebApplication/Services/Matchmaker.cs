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
        private IGamePerformer _gamePerformer; 
        private ILogger<Matchmaker> _logger; 

        public Matchmaker(IGamePerformer gamePerformer, ILoggerFactory loggerFactory)
        {
            _gamePerformer = gamePerformer;
            _logger = loggerFactory.CreateLogger<Matchmaker>();
        }
        public void StartAISesionAsync(IUserDto user)
        {
            //I used Task.Run instead of TaskFactory.StartNew to reuse threads in thread pool 
            Task.Run(async () =>
            {
                user.EnterGame();
                bool timeoutHasCome = false;
                var timer = new Timer(Callback => { timeoutHasCome = true; }, null, 300_000,
                    Timeout.Infinite);
                var session = new Session(user.Account.Login, "computer");
                while (true)
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

                        var round = await _gamePerformer.StartRoundWithAIAsync(user, 
                            user.CurrentGame().Token, 
                            new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);
                        

                        if (round != null) session.Rounds.Add(round);
                    }
                    catch (TaskCanceledException e)
                    {
                        user.ResetReady();
                        user.ExitGame();

                        if (session.Rounds.Count == 0) return;
                        session.EndingReason = "user quited session";
                        user.RegisterNewSession(session);
                        return;
                    }
                    catch (TimeoutException)
                    {
                        continue;
                    }
                }
            });
        }

        public void StartRegularSesionAsync(IUserDto user1, IUserDto user2)
        {
            Task.Run(async () =>
            {
                user1.EnterGame();
                user2.EnterGame();
                bool timeoutHasCome = false;
                var timer = new Timer(Callback => { timeoutHasCome = true; }, null, 300_000,
                    Timeout.Infinite);
                var session = new Session(user1.Account.Login, user2.Account.Login);
                while (true)
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
                            user1.CurrentGame().Token, user2.CurrentGame().Token,
                            new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);

                        if (round != null) session.Rounds.Add(round);
                    }
                    catch (OperationCanceledException e)        // maybe here
                    {
                        user1.ResetReady();
                        user2.ResetReady();
                        user1.ExitGame();
                        user2.ExitGame();

                        if (session.Rounds.Count == 0) return;
                        session.EndingReason = "user quited session";
                        user1.RegisterNewSession(session);
                        user2.RegisterNewSession(session);
                        return;
                    }
                    catch (TimeoutException)
                    {
                        continue;
                    }
                    user1.ResetReady();
                    user2.ResetReady();
                }
            });
        }
    }
}
