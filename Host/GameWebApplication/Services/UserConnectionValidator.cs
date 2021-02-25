using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class UserConnectionValidator : BackgroundService
    {
        private List<IUserDto> _users;

        public UserConnectionValidator(IUserStorage userStorage)
        {
            _users = userStorage.GetUsers();
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
            _users.AsParallel().Where(u => u.IsActive()).ForAll(u =>
            {
                if (!u.CheckForConnection())
                {
                    u.Disactivate();
                }
                else
                {
                    u.ResetConnection();
                }
            });
        }
    }
}
