using BusinessLogic.Services;

namespace BlazorWeb.Server.HangfireJobs
{
    public interface IHangfireDashboardIncomingCallsRun
    {
        Task Run();
    }

    public class HangfireDashboardIncomingCallsRun : IHangfireDashboardIncomingCallsRun
    {
        private readonly IAccountServices _accountServices;
        private readonly ITwilioServices _twilioServices;

        public HangfireDashboardIncomingCallsRun(ITwilioServices twilioServices,
            IAccountServices accountServices)
        {
            _twilioServices = twilioServices;
            _accountServices = accountServices;
        }
        public async Task Run()
        {
            var accounts = await _accountServices.GetAllActiveAccounts();
            foreach (var account in accounts)
            {
                await _twilioServices.RetrieveCallLogs(account);
            }
        }
    }
}
