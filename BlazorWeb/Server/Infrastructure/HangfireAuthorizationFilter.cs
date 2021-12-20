using Domain.Models;
using Hangfire.Dashboard;

namespace BlazorWeb.Server.Infrastructure
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole(Constants.RoleAdmin) || 
                   httpContext.User.IsInRole(Constants.RoleDeveloper);
        }
    }
}