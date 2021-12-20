namespace BlazorWeb.Server.HangfireJobs
{
    public interface IHangfireSample
    {
        Task Run(string accountSid);
    }

    public class HangfireSample : IHangfireSample
    {
        public async Task Run(string accountSid)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            await Task.Delay(1000);
        }
    }
}
