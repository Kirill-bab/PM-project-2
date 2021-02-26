using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class Statistics : IStatistics
    {
        public List<Session> SessionsList { get; set; }
        public string TotalTimeInGame { get; set; }
        public Statistics()
        {
            TotalTimeInGame = TimeSpan.Zero.ToString();
        }
    }
}

