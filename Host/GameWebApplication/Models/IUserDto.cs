using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public interface IUserDto
    {
        public UserAccount Account { get; set; }
        public string LastRoundResult { get; set; }
        public void Activate();
        public void Disactivate();
        public void RegisterNewSession(Session session);
        public Figure GetCurrentFigure();
        public void ChangeCurrentFigure(Figure figure);
        public bool CheckForConnection();    // for permanent connection Checks
        public void Connect();
        public void ResetConnection();
        public bool IsActive();
        public void EnterGame();
        public void ExitGame();
        public bool IsInGame();
        public bool IsInQueue { get; set; }
        public void ResetCancellationToken();
        public void StartRound();
        public void EndRound();
        public bool IsInRound();
        public CancellationTokenSource CurrentGame();
    }
}
