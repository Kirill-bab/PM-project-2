using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class UserAccount : IUserAccount
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public IStatistics Statistics { get; set; }
        public string TimeInGame { get; set; }

        public UserAccount()
        {

        }
        public UserAccount(string login, string password, IStatistics statistics, string timeInGame)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
            TimeInGame = timeInGame ?? throw new ArgumentNullException(nameof(timeInGame));
        }
    }
}
