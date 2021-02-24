using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    interface IUserDto
    {
        public IUserAccount Account { get; set; }
        public IStatistics Statistics { get; set; }

        public void Activate();
    }
}
