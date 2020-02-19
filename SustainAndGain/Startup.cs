using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SustainAndGain.Models;
using SustainAndGain.Models.Entities;
using SustainAndGain.Models.Scheduling;

namespace SustainAndGain
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            // Adding QurtzJobrunner to be able to use scoped services(writing to db)
            services.AddSingleton<QuartzJobRunner>();
            //Adding our jobs
            services.AddScoped<TriggerGetStockPricesJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(TriggerGetStockPricesJob),
                cronExpression: "0 5 9,13,18 ? * MON,TUE,WED,THU,FRI *")); // run Monday through Friday, at 0905, 1305, and 1805
            services.AddScoped<TriggerExecuteOrdersJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(TriggerExecuteOrdersJob),
                cronExpression: "0 7 9,13,18 ? * MON,TUE,WED,THU,FRI *")); // run Monday through Friday, at 0907, 1307, and 1807
            services.AddScoped<TriggerUpdateCurrentPortfolioValues>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(TriggerUpdateCurrentPortfolioValues),
                cronExpression: "0 8 9,13,18 ? * MON,TUE,WED,THU,FRI *")); // run Monday through Friday, at 0908, 1308, and 1808

            services.AddHostedService<QuartzHostedService>();

            var connString = configuration.GetConnectionString("DefaultConnection");
            services.AddControllersWithViews();

            services.AddDbContext<SustainGainContext>(o => o.UseSqlServer(connString));
            services.AddDbContext<MyIdentityContext>(o => o.UseSqlServer(connString));
            services.AddIdentity<MyIdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            }
                )
                .AddEntityFrameworkStores<MyIdentityContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(o => o.LoginPath = "/login");
            services.AddTransient<UsersService>();
            services.AddTransient<StocksService>();
            services.AddTransient<PortfoliosService>();
            services.AddTransient<CompetitionsService>();
            services.AddHttpContextAccessor();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
                app.UseExceptionHandler("/Error/ServerError");

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}

