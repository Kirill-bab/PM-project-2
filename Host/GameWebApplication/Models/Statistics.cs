﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class Statistics : IStatistics
    {
        public List<Session> GamesList { get; set; }
        public int MyProperty { get; set; }
    }
}
