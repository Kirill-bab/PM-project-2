using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class PrivateSession : Session
    {
        public Guid GameKey { get; private set; }

        public PrivateSession(string player1, string player2) : base( player1, player2)
        {
            GameKey = Guid.NewGuid();
        }
        
        public override string ToString()
        {
            return base.ToString() + TableBuilder.AlignCentre(GameKey.ToString(), 20);
        }
    }
}
