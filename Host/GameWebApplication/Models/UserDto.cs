using System;
using System.Collections.Generic;
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
        
        private bool _isActive;
        private readonly Figure _currentFigure;

        public UserDto()
        {
            IsActive = false;
            _currentFigure = Figure.None;
        }

        public UserDto(IUserAccount account)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            IsActive = false;
            _currentFigure = Figure.None;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
            }
        }
        public Figure GetCurrentFigure()
        {
            return _currentFigure;
        }
        public void RegisterNewSession(Session session)
        {
            if (session.Rounds.Count == 0) return;

            Account.Statistics.GamesList.Add(session);
        }

        public void Disactivate()
        {
            IsActive = false;
        }
        public Figure GetCurrentFigure()
        {
            return _currentFigure;
        }
        public void RegisterNewSession(Session session)
        {
            if (session.Rounds.Count == 0) return;

            Account.Statistics.GamesList.Add(session);
        }
    }
}
