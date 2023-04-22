using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DigiSeatApi.Interfaces;
using DigiSeatApi.Application;
using DigiSeatApi.DataAccess;
using DigiSeatApi.Composition;
using Microsoft.AspNetCore.Http;

namespace DigiSeatApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private ILoggerFactory _loggerFactory { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var builder = services.AddMvc();
            builder.AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter(_loggerFactory)); });
            services.AddCors();

            //Add Application Services
            services.AddTransient<ITableManager, TableManager>();
            services.AddSingleton<IDigiSeatRepo, DigiSeatRepo>();
            services.AddTransient<IWaitManager, WaitManager>();
            services.AddTransient<IRestaurantManager, RestaurantManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseBrowserLink();
            app.UseMvcWithDefaultRoute();

            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:4200"));
        }
    }
}
