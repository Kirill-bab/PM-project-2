using System.Collections.Generic;
using Library;


namespace ConsoleClient.Models
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

        public override string ToString()
        {
            return Player1.PadRight(15) + Player2.PadRight(15) + StartedAt;
        }
    }
}
