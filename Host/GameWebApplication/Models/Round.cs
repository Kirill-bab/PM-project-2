using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class Round
    {
        public (string, Figure) Winner { get; set; }
        public (string, Figure) Looser { get; set; }
    }
    public enum Figure
    {
        Paper,
        Rock,
        Scissors
            //we can add more)
    }
}
