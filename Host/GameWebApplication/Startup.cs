using GameWebApplication.Abstractions;
using GameWebApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                // Disable auto BadRequest response
                options.SuppressModelStateInvalidFilter = true;
                // Disable auto error explanations 
                options.SuppressMapClientErrors = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameWebApplication", Version = "v1" });
            });

            services.AddTransient<ILoggerFactory, LoggerFactory>();
            services.AddHostedService<StorageWorker>();
            services.AddHostedService<UserConnectionValidator>();
            services.AddSingleton<IUserStorage, UserStorage>();
            services.AddTransient<IGamePerformer, GamePerformer>();
            services.AddTransient<IMatchmaker, Matchmaker>();
            services.AddSingleton<IGamingPlatform, GamingPlatform>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameWebApp;ication v1"));
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
