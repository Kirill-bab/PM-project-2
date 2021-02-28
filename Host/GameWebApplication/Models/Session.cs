using System;
using System.Collections.Generic;
using System.Diagnostics;
using Library;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class Session
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string  EndingReason { get; set; }
        public List<Round> Rounds { get; set; }
        public string StartedAt { get; set; }

        public Session()
        {

        }
        public Session(string player1, string player2)
        {
            Player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
            Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
            Rounds = new List<Round>(5);
            StartedAt = new DateTime(DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second).ToString();
        }

        public override string ToString()
        {
            return TableBuilder.AlignCentre(Player1, 20) + TableBuilder.AlignCentre(Player2, 20) +
                TableBuilder.AlignCentre(Rounds.Count.ToString(), 5);
        }
    }
}
