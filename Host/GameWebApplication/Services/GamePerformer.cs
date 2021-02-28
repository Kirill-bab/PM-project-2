using System;
using GameWebApplication.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using GameWebApplication.Models;
using Microsoft.Extensions.Logging;

namespace GameWebApplication.Services
{
    public class GamePerformer : IGamePerformer
    {
        private readonly ILogger<GamePerformer> _logger;

        public GamePerformer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GamePerformer>();
        }
        public async Task<Round> StartRoundWithPlayerAsync(IUserDto user1, IUserDto user2, 
            CancellationToken user1Ct, CancellationToken user2Ct, CancellationToken timeoutCt)
        {
            return await Task.Run(async () =>
            {
                user1.StartRound();
                user2.StartRound();

                _logger.LogWarning($"Waiting for users {user1.Account.Login}" +
                        $" and {user2.Account.Login} turns");
                while (user1.GetCurrentFigure() == Figure.None ||
                user2.GetCurrentFigure() == Figure.None)
                {
                    
                    if (user1Ct.IsCancellationRequested || user2Ct.IsCancellationRequested)
                    {
                        user1.EndRound();
                        user2.EndRound();
                        return default;
                    }

                    if (timeoutCt.IsCancellationRequested) return default;
                }
                _logger.LogWarning($"Users {user1.Account.Login} " +
                    $"and {user2.Account.Login} have chosen figures {user1.GetCurrentFigure()}" +
                    $"and {user2.GetCurrentFigure()}!");

                var result = CheckForWinner(user1.GetCurrentFigure(), user2.GetCurrentFigure());

                user1.EndRound();
                user2.EndRound();

                await Task.Delay(1000);
                switch (result)
                {
                    case 1:
                        {
                            _logger.LogWarning($"Player {user1.Account.Login} won!");
                            user1.LastRoundResult = "victory";
                            user2.LastRoundResult = "defeat";
                            return new Round
                            {
                                Winner = (user1.Account.Login),
                                Looser = user2.Account.Login,
                                WinnerFigure = user1.GetCurrentFigure(),
                                LooserFigure = user2.GetCurrentFigure()
                            };
                        }
                    case 2:
                        {
                            _logger.LogWarning($"Player {user2.Account.Login} won!");
                            user1.LastRoundResult = "defeat";
                            user2.LastRoundResult = "victory";
                            return new Round
                            {
                                Winner = (user2.Account.Login),
                                Looser = user1.Account.Login,
                                WinnerFigure = user2.GetCurrentFigure(),
                                LooserFigure = user1.GetCurrentFigure()
                            };
                        }
                    case 0:
                        {
                            user1.LastRoundResult = "draw";
                            user2.LastRoundResult = "draw";
                            return new Round
                            {
                                Winner = ("draw"),
                                Looser = ("draw"),
                                WinnerFigure = Figure.None,
                                LooserFigure = Figure.None
                            };
                        }
                }
               

                return default;
            }, timeoutCt);
        }

        public async Task<Round> StartRoundWithAIAsync(IUserDto user, CancellationToken ct, CancellationToken timeoutCt)
        {
            return await Task.Run(async () =>
            {
                user.StartRound();

                _logger.LogWarning($"Waiting for user {user.Account.Login}" +
                        $"turn");
                while (user.GetCurrentFigure() == Figure.None)
                {

                    if (ct.IsCancellationRequested)
                    {
                        user.EndRound();
                        
                        return default;
                    }

                    if (timeoutCt.IsCancellationRequested) return default;
                }
                var aiFigure = new AIPlayer().GetRandomFigure();

                _logger.LogWarning($"User {user.Account.Login} " +
                    $"and AI have chosen figures {user.GetCurrentFigure()}" +
                    $"and {aiFigure}!");

                var result = CheckForWinner(user.GetCurrentFigure(), aiFigure);

                user.EndRound();

                await Task.Delay(1000);
                switch (result)
                {
                    case 1:
                        {
                            _logger.LogWarning($"Player {user.Account.Login} won!");
                            user.LastRoundResult = "victory";
                            return new Round
                            {
                                Winner = (user.Account.Login),
                                Looser = "computer",
                                WinnerFigure = user.GetCurrentFigure(),
                                LooserFigure = aiFigure
                            };
                        }
                    case 2:
                        {
                            _logger.LogWarning($"Computer won!");
                            user.LastRoundResult = "defeat";
                            return new Round
                            {
                                Winner = "computer",
                                Looser = user.Account.Login,
                                WinnerFigure = aiFigure,
                                LooserFigure = user.GetCurrentFigure()
                            };
                        }
                    case 0:
                        {
                            user.LastRoundResult = "draw";
                            
                            return new Round
                            {
                                Winner = ("draw"),
                                Looser = ("draw"),
                                WinnerFigure = Figure.None,
                                LooserFigure = Figure.None
                            };
                        }
                }
                return default;
            }, timeoutCt);
        }

        private int CheckForWinner(Figure figure1, Figure figure2)
        {
            if (figure1.Equals(figure2)) return 0;

            switch (figure1)
            {
                case Figure.Paper:
                    {
                        if (figure2 == Figure.Rock) return 1;
                        else return 2;
                    }
                case Figure.Rock:
                    {
                        if (figure2 == Figure.Scissors) return 1;
                        else return 2;
                    }
                case Figure.Scissors:
                    {
                        if (figure2 == Figure.Paper) return 1;
                        else return 2;
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
