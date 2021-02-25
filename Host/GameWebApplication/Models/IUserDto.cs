using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public interface IUserDto
    {
        public IUserAccount Account { get; set; }
        public void Activate();
        public void Disactivate();
        public void RegisterNewSession(Session session);
        public Figure GetCurrentFigure();
        public bool IsReadyForNextRound();
        public bool CheckForConnection();
        public void Connect();
        public void ResetConnection();
        public bool IsActive();
    }
}
