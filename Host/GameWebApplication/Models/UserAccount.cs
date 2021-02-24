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

        public UserAccount()
        {

        }
        public UserAccount(string login, string password)
        {
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
