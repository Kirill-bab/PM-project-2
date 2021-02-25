using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public interface IUserDto
    {
        public IUserAccount Account { get; set; }
        public void Activate();
        public void Disactivate();
        public Figure GetCurrentFigure();
    }
}
