using System;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            new SessionPerformer(new Models.UserAccount()).Run();
        }
    }
}
