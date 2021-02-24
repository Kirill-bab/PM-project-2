using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication.Models
{
    public class UserDto : IUserDto
    {
        public IUserAccount Account { get; set; }
        public IStatistics Statistics { get; set; }
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }
        [NonSerialized]
        private bool _isActive;

        public UserDto()
        {
            IsActive = false;
        }

        public UserDto(IUserAccount account, IStatistics statistics)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
