using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class AIPlayer
    {
        public Figure GetRandomFigure()
        {
            return (Figure)Enum.GetValues(typeof(Figure)).GetValue(new Random().Next(1,4));
        }
    }
}
