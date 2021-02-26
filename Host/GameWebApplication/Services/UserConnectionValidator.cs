using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class UserConnectionValidator : BackgroundService
    {
        private readonly List<IUserDto> _users;
        private readonly ILogger<UserConnectionValidator> _logger;
        private readonly IGamingPlatform _gamingPlatform;

        public UserConnectionValidator(IUserStorage userStorage, ILoggerFactory loggerFactory,
            IGamingPlatform gamingPlatform)
        {
            _gamingPlatform = gamingPlatform;
            _users = userStorage.GetUsers();
            _logger = loggerFactory.CreateLogger<UserConnectionValidator>();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var timer = new Timer(Callback, null, 0, 3000);
                while (true)
                {
                    if (stoppingToken.IsCancellationRequested) break;
                }
            }, stoppingToken);
        }

        private void Callback(object state)
        {
            _users.AsParallel().Where(u => u.IsActive()).ForAll(async u =>
            {
                if (!u.CheckForConnection())
                {
                    await _gamingPlatform.DisconnectUserAsync(u.Account.Login);
                }
                else
                {
                    u.ResetConnection();
                }
            });
        }
    }
}
