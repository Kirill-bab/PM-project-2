using System;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GameWebApplication.Models
{
    public class UserAccount : IUserAccount
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        public string Password { get; set; }
        public Statistics Statistics { get; set; }

        public UserAccount()
        {

        }
        public UserAccount(string login, string password)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Statistics = new Statistics();
        }
        public UserAccount(string login, string password, Statistics statistics, string timeInGame)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        }
    }
}
