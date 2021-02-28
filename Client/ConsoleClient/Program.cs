using System;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                new SessionPerformer(new Models.UserAccount()).Run();
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Sorry, Unexpected error occured!");
                Console.ReadKey(true);
            }
        }
    }
}
