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
      
        public Session()
        {

        }

        public override string ToString()
        {
            return TableBuilder.AlignCentre(Player1, 20) + TableBuilder.AlignCentre(Player2, 20) +
                TableBuilder.AlignCentre(EndingReason, 5);
        }
    }
}
