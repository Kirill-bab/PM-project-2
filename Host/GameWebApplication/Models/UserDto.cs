using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class UserDto : IUserDto
    {
        public UserAccount Account { get; set; }
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }
        public bool IsInQueue { get; set; }

        private bool _isActive;
        private Figure _currentFigure;
        private readonly Stopwatch _currentPeriodTime;
        private bool _isConnected;
        private CancellationTokenSource _currentGame;
        private bool _isInGame;
        private bool _isInRound;
        private TimeSpan _userTimeInGame => TimeSpan.Parse(this.Account.Statistics.TotalTimeInGame);

        public string LastRoundResult { get; set; }

        public UserDto()
        {
            _currentGame = new CancellationTokenSource();
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isConnected = false;
            _isInGame = false;
            IsInQueue = false;
        }

        public UserDto(UserAccount account)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isConnected = false;
            _currentGame = new CancellationTokenSource();
            _isInGame = false;
            IsInQueue = false;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                _currentPeriodTime.Start();
                IsInQueue = false;
                Console.WriteLine($"user {Account.Login} activated!");
                _isConnected = true;
            }
        }
        public Figure GetCurrentFigure()
        {
            return _currentFigure;
        }
        public void RegisterNewSession(Session session)
        {
            if (session.Rounds.Count == 0) return;

            Account.Statistics.SessionsList.Add(session);
        }

        public bool CheckForConnection()
        {
            return _isConnected;
        }

        public void Disactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                _currentPeriodTime.Stop();
                this.Account.Statistics.TotalTimeInGame = (_currentPeriodTime.Elapsed + _userTimeInGame).ToString();
                _isConnected = false;
                _isInGame = false;
                _currentGame.Cancel();
                //_currentGame.Dispose();
                Console.WriteLine($"user {Account.Login} disactivated!"); 
            }
        }

        public void Connect()
        {
            _isConnected = true;
        }

        public void ResetConnection()
        {
            _isConnected = false;
        }

        bool IUserDto.IsActive()
        {
            return IsActive;
        }

        public void ChangeCurrentFigure(Figure fig)
        {
            _currentFigure = fig;
        }

        public CancellationTokenSource CurrentGame()
        {
            return _currentGame;
        }

        public void ResetCancellationToken()
        {
            _currentGame.Cancel();
            //_currentGame.Dispose();
            _currentGame = new CancellationTokenSource();
        }

        public void EnterGame()
        {
            _isInGame = true;
        }

        public void ExitGame()
        {
            _currentGame.Cancel();
            //_currentGame.Dispose();
            _isInGame = false;
        }

        public bool IsInGame()
        {
            return _isInGame;
        }

        public void StartRound()
        {
            _isInRound = true;
        }

        public void EndRound()
        {
            _isInRound = false;
        }

        public bool IsInRound()
        {
            return _isInRound;
        }
    }
}
