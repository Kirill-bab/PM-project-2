﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public interface IUserAccount
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Statistics Statistics { get; set; }
    }
}
