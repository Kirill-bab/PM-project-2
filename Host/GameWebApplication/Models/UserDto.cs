using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class UserDto : IUserDto
    {
        public IUserAccount Account { get; set; }
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
        private readonly Figure _currentFigure;
        private Stopwatch _currentPeriodTime;
        private bool _isConnected;
        private TimeSpan _userTimeInGame => TimeSpan.Parse(this.Account.TimeInGame);

        public UserDto()
        {
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isReadyForNextRound = false;
            _isConnected = false;
        }

        public UserDto(IUserAccount account)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            IsActive = false;
            _currentFigure = Figure.None;
            _currentPeriodTime = new Stopwatch();
            _isReadyForNextRound = false;
            _isConnected = false;
        }
        public void SetReady()
        {
            _isReadyForNextRound = true;
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
    }
}
