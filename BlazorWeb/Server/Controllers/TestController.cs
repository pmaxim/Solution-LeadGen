using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = $"{Constants.RoleAdmin},{Constants.RoleAgent}")]
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITwilioWebhookRepository _twilioWebhookRepository;

        public TestController(ITwilioWebhookRepository twilioWebhookRepository)
        {
            _twilioWebhookRepository = twilioWebhookRepository;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTwilioWebhooks()
        {
            return Ok(
                await _twilioWebhookRepository.TwilioWebhooks.AsNoTracking()
                    .OrderByDescending(z=>z.DateTime)
                    .Take(1000)
                    .ToListAsync()
            );
        }
    }
}
