using AutoMapper;
using BlazorWeb.Server.Hubs;
using BlazorWeb.Shared.Models;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [ApiController]
    public class WebhookTwilioController : ControllerBase
    {
        private readonly ITwilioWebhookRepository _twilioWebhookRepository;
        private readonly IMapper _mapper;
        private readonly ITalkActive _hubContext;

        public WebhookTwilioController(ITwilioWebhookRepository twilioWebhookRepository,
            ITalkActive hubContext, IMapper mapper)
        {
            _twilioWebhookRepository = twilioWebhookRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        //https://leadgen.serverpipe.com/twilioStatus
        //https://localhost:7104/twilioStatus
        [Route("twilioStatus")]
        [HttpGet]
        public async Task<ActionResult> Status(string CallSid, string To, 
            string Caller, string CallStatus, string Direction)
        {
            _twilioWebhookRepository.Create(new TwilioWebhook
            {
                CallSid = CallSid, To = To, From = Caller, 
                Status = CallStatus, Direction = Direction
            });
            await _twilioWebhookRepository.SaveChangesAsync();

            await _hubContext.IncomingVoiceEvent(new WebhookVoiceModel
            {
                Sid = CallSid, To = To, From = Caller, Status = CallStatus,
                Name = ShareConstants.Twilio
            });

            return Ok();
        }
    }
}
