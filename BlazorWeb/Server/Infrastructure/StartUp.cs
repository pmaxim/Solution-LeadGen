using BlazorWeb.Server.HangfireJobs;
using BlazorWeb.Server.Hubs;
using BlazorWeb.Server.Models;
using BusinessLogic.Services;
using Domain.Models;
using Domain.Repositories;
using Domain.Repositories.Concrete;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RazorHtmlEmails.RazorClassLib.Services;

namespace BlazorWeb.Server.Infrastructure
{
    public static class StartUp
    {
        public static void ConfigureDi(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //repo
            services.AddScoped<ISettingRepository, EfSettingRepository>();
            services.AddScoped<IAccountRepository, EfAccountRepository>();
            services.AddScoped<IVonageWebhookRepository, EfVonageWebhookRepository>();
            services.AddScoped<ITwilioWebhookRepository, EfTwilioWebhookRepository>();
            services.AddScoped<ILeadPhoneRepository, EfLeadPhoneRepository>();
            services.AddScoped<IAccountApplicationRepository, EfAccountApplicationRepository>();
            services.AddScoped<INotificationSubscriptionRepository, EfNotificationSubscriptionRepository>();
            services.AddScoped<IScheduleRepository, EfScheduleRepository>();
            services.AddScoped<IDashboardIncomingCallRepository, EfDashboardIncomingCallRepository>();
            services.AddScoped<ICallIncomingRepository, EfCallIncomingRepository>();

            //services
            services.AddScoped<IVonageServices, VonageServices>();
            services.AddScoped<ITwilioServices, TwilioServices>();
            services.AddScoped<ISettingServices, SettingServices>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<INotificationServices, NotificationServices>();
            services.AddScoped<IScheduleServices, ScheduleServices>();
            services.AddScoped<IDashboardServices, DashboardServices>();

            //hangfire
            services.AddScoped<IHangfireSample, HangfireSample>();
            services.AddScoped<IHangfireRun, HangfireRun>();
            services.AddScoped<IHangfireScheduleRun, HangfireScheduleRun>();
            services.AddScoped<IHangfireDashboardIncomingCallsRun, HangfireDashboardIncomingCallsRun>();

            //SignalR
            services.AddScoped<ITalkActive, TalkActive>();

            //email AddTransient - if AddScoped - error for ISettingRepository 
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        }

        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            //Resolve ASP .NET Core Identity with DI help
            var userManager = (UserManager<ApplicationUser>)
                scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))!;
            var roleManager = (RoleManager<IdentityRole>)
                scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))!;

            var adminRoleExists = await roleManager.RoleExistsAsync(Constants.RoleAdmin);

            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(Constants.RoleAdmin));
            }

            var agentRoleExists = await roleManager.RoleExistsAsync(Constants.RoleAgent);

            if (!agentRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(Constants.RoleAgent));
            }

            var developerRoleExists = await roleManager.RoleExistsAsync(Constants.RoleDeveloper);

            if (!developerRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(Constants.RoleDeveloper));
            }

            var userToMakeAdmin = await userManager.FindByNameAsync("admin@admin.net");
            if (userToMakeAdmin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@admin.net", Email = "admin@admin.net", EmailConfirmed = true,
                    LockoutEnabled = false
                };
                var result = await userManager.CreateAsync(user, "Z2030r###");
                if (!result.Succeeded) return;
                await userManager.AddToRoleAsync(user, Constants.RoleAdmin);
            }

            var userToMakeDeveloper = await userManager.FindByNameAsync("nc@ukr.net");
            if (userToMakeDeveloper == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "nc@ukr.net",
                    Email = "nc@ukr.net",
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };
                var result = await userManager.CreateAsync(user, "Max1967!");
                if (!result.Succeeded) return;
                await userManager.AddToRoleAsync(user, Constants.RoleDeveloper);
                await userManager.AddToRoleAsync(user, Constants.RoleAdmin);
            }

            //var userPmx = await userManager.FindByNameAsync("pmx@ukr.net");
            //if (userPmx != null)
            //{
            //    await userManager.DeleteAsync(userPmx);
            //}
        }

        public static void AddHangfireJob(WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
                IgnoreAntiforgeryToken = true
            });
            
            RecurringJob.AddOrUpdate<IHangfireScheduleRun>(z => z.Run(), Cron.Minutely);

            //RecurringJob.AddOrUpdate<IHangfireSample>(z => z.Run("1"), Cron.Minutely);

            //var jobId = BackgroundJob.Enqueue<IHangfireRun>(z => z.Run("admin@admin.net"));
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadSendSms>(z => z.Run(), "*/2 * * * *");
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadDisposition>(z => z.Run(), "*/5 * * * *");
            //RecurringJob.AddOrUpdate<IHangfireFacebookLeadAlarm>(z => z.Run(), Cron.Minutely);
            RecurringJob.AddOrUpdate<IHangfireDashboardIncomingCallsRun>(z => z.Run(), Cron.Daily);
        }
    }
}
