using System;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ConsoleClient.Models
{
    public class UserAccount
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        public string Password { get; set; }
        public Statistics Statistics { get; set; }

        public UserAccount()
        {

        }
    }
}
