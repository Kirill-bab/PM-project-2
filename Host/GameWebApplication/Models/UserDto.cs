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
        private bool _isReadyForNextRound;
        private bool _isActive;
        private Figure _currentFigure;
        private readonly Stopwatch _currentPeriodTime;
        private bool _isConnected;
        private CancellationTokenSource _currentGame;
        private bool _isInGame;
        private TimeSpan _userTimeInGame => TimeSpan.Parse(this.Account.TimeInGame);

        public UserDto()
        {
            _currentGame = new CancellationTokenSource();
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isReadyForNextRound = false;
            _isConnected = false;
            _isInGame = false;
        }

        public UserDto(UserAccount account)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isReadyForNextRound = false;
            _isConnected = false;
            _currentGame = new CancellationTokenSource();
            _isInGame = false;
        }
        public void SetReady()
        {
            _isReadyForNextRound = true;
        }

        public void ResetReady()
        {
            _isReadyForNextRound = false;
            ResetCancellationToken();
        }
        public bool IsReadyForNextRound()
        {
            return _isReadyForNextRound;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                _currentPeriodTime.Start();
                _isReadyForNextRound = true;
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
                this.Account.TimeInGame = (_currentPeriodTime.Elapsed + _userTimeInGame).ToString();
                _isReadyForNextRound = false;
                _isConnected = false;
                _isInGame = false;
                _currentGame.Cancel();
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
            SetReady();
        }

        public CancellationTokenSource CurrentGame()
        {
            return _currentGame;
        }

        private void ResetCancellationToken()
        {
            _currentGame = new CancellationTokenSource();
        }

        public void EnterGame()
        {
            _isInGame = true;
        }

        public void ExitGame()
        {
            _isInGame = false;
        }

        public bool IsInGame()
        {
            return _isInGame;
        }
    }
}
