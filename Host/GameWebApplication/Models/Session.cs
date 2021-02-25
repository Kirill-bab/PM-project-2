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
        public List<Round> Rounds { get; set; }
        public Stopwatch Duration { get; set; }
        //public bool IsFinished { get; set; }

        public Session()
        {

        }
        public Session(string player1, string player2)
        {
            Player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
            Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
            Rounds = new List<Round>(5);
            Duration = Stopwatch.StartNew();
        }

        public override string ToString()
        {
            return TableBuilder.AlignCentre(Player1, 20) + TableBuilder.AlignCentre(Player2, 20) +
                TableBuilder.AlignCentre(Rounds.Count.ToString(), 5) +
                TableBuilder.AlignCentre(Duration.Elapsed.ToString(), 20);
        }
    }
}
