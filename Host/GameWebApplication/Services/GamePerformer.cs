using System;
using GameWebApplication.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using GameWebApplication.Models;


namespace GameWebApplication.Services
{
    public class GamePerformer : IGamePerformer
    {
        private static readonly TaskFactory _factory = new TaskFactory();

        public Task<Round> StartRoundWithPlayerAsync(IUserDto user1, IUserDto user2, 
            CancellationToken ct, CancellationToken timeoutCt)
        {
            return _factory.StartNew<Round>(() =>
            {
                while (user1.GetCurrentFigure() == Figure.None ||
                user2.GetCurrentFigure() == Figure.None)
                {
                    ct.ThrowIfCancellationRequested();
                    if (timeoutCt.IsCancellationRequested) throw new TimeoutException(nameof(timeoutCt));
                }

                var result = CheckForWinner(user1.GetCurrentFigure(), user2.GetCurrentFigure());

                switch (result)
                {
                    case 1:
                        return new Round
                        {
                            Winner = (user1.Account.Login, user1.GetCurrentFigure()),
                            Looser = (user2.Account.Login, user2.GetCurrentFigure())
                        };
                    case 2:
                        return new Round
                        {
                            Winner = (user2.Account.Login, user2.GetCurrentFigure()),
                            Looser = (user1.Account.Login, user1.GetCurrentFigure())
                        };
                    case 0:
                        return new Round
                        {
                            Winner = ("draw", user1.GetCurrentFigure()),
                            Looser = ("draw", user2.GetCurrentFigure())
                        };
                }
                return default;
            }, ct);
        }

        public Task<Round> StartRoundWithAIAsync(IUserDto user, CancellationToken ct, CancellationToken timeoutCt)
        {
            return _factory.StartNew<Round>(() =>
            {
                var aiFigure = new AIPlayer().GetRandomFigure();
                while (user.GetCurrentFigure() == Figure.None)
                {
                    ct.ThrowIfCancellationRequested();
                    if (timeoutCt.IsCancellationRequested) throw new TimeoutException(nameof(timeoutCt));
                }

                var result = CheckForWinner(user.GetCurrentFigure(), aiFigure);

                switch (result)
                {
                    case 1:
                        return new Round
                        {
                            Winner = (user.Account.Login, user.GetCurrentFigure()),
                            Looser = ("computer", aiFigure)
                        };
                    case 2:
                        return new Round
                        {
                            Winner = ("computer", aiFigure),
                            Looser = (user.Account.Login, user.GetCurrentFigure())
                        };
                    case 0:
                        return new Round
                        {
                            Winner = ("draw", default(Figure)),
                            Looser = ("draw", default(Figure))
                        };
                }
                return default;
            }, ct);
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
