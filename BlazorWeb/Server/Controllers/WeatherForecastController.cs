using BlazorWeb.Server.HangfireJobs;
using BlazorWeb.Server.Models;
using BlazorWeb.Shared;
using BusinessLogic.Services;
using Domain.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = Constants.RoleAdmin)]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVonageServices _vonageServices;
        private readonly IHangfireSample _hangfireSample;

        public WeatherForecastController(
            IVonageServices vonageServices,
            IHangfireSample hangfireSample,
            ILogger<WeatherForecastController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _vonageServices = vonageServices;
            _hangfireSample = hangfireSample;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var t = _userManager.GetUserName(User);

            //var vonageNumbersApi = await _vonageServices.ApiGetPhoneNumbers(2);

            var vonageNumbers = await _vonageServices.GetPhoneNumbers(2);
            //await _vonageServices.CallNumber(2, vonageNumbers.Numbers[0].Msisdn, "380679223773");

            //var app = await _vonageServices.GetApplication(2);

            //var apps = await _vonageServices.GetApplications(2);

            //var numbers = await _vonageServices.GetApplicationNumber(2);

            var jobId = BackgroundJob.Enqueue(() => _hangfireSample.Run("2"));

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}