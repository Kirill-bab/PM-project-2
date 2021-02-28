using GameWebApplication.Abstractions;
using GameWebApplication.Models;
using Library;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GameWebApplication.Services
{
    public class StorageWorker : IHostedService
    {
        private readonly ILogger<StorageWorker> _logger;
        private readonly IUserStorage _storage;
        public StorageWorker(ILoggerFactory loggerFactory, IUserStorage userStorage)
        {
            _logger = loggerFactory.CreateLogger<StorageWorker>();
            _storage = userStorage;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _storage.InitializeUserList(FileWorker.Read("users.json"));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            FileWorker.Write("users.json", JsonSerializer
                .Serialize<List<UserAccount>>(_storage
                .GetUsers().Select(u => u.Account).ToList(), new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            
            _logger.LogWarning("USER STORAGE SAVED!");

            return Task.CompletedTask;
        }
    }
}
