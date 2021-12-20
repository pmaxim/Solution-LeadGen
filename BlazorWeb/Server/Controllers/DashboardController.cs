using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = $"{Constants.RoleAdmin},{Constants.RoleAgent}")]
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardServices _dashboardServices;
        public DashboardController(IDashboardServices dashboardServices)
        {
            _dashboardServices = dashboardServices;
        }

        [HttpPost("[action]")]
        public async Task<DashboardIncomingCallsViewModel> GetTableIncomingCalls(FilterDashboardModel filter)
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return new DashboardIncomingCallsViewModel();
            
            var role = string.Empty;
            if (User.IsInRole(Constants.RoleAdmin)) role = Constants.RoleAdmin;

            await _dashboardServices.RetrieveCallLogs(userName, role);

            return await _dashboardServices.GetTableIncomingCalls(filter, userName, role);
        }
    }

}
