using GameWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public interface IStatistics
    {
        public List<Session> SessionsList { get; set; }
        public string TotalTimeInGame{ get; set; }
    }
}
