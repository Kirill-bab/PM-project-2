using GameWebApplication.Models;
using Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //FileWorker.Write("users.json", JsonSerializer.Serialize(new List<UserAccount>()));
            CreateHostBuilder(args).Build().Run();    
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Information);
                    loggingBuilder.AddSerilog(new LoggerConfiguration() 
                        .WriteTo.Console()
                        .WriteTo.File("app.log")
                        .CreateLogger());
                });
    }
}
