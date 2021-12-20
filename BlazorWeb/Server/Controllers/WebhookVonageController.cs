using AutoMapper;
using BlazorWeb.Server.Hubs;
using BlazorWeb.Server.Models;
using BlazorWeb.Shared.Models;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [ApiController]
    public class WebhookVonageController : ControllerBase
    {
        private readonly IVonageWebhookRepository _vonageWebhookRepository;
        private readonly IMapper _mapper;
        private readonly ITalkActive _hubContext;

        public WebhookVonageController(IVonageWebhookRepository vonageWebhookRepository,
            ITalkActive hubContext, IMapper mapper)
        {
            _vonageWebhookRepository = vonageWebhookRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        //https://leadgen.serverpipe.com/vonageVoice
        //https://localhost:7104/vonageVoice
        //https://developer.vonage.com/voice/voice-api/webhook-reference#event-webhook
        [Route("vonageVoice")]
        [HttpPost]
        public async Task<ActionResult> Voice(VonageVoiceEndPoint m)
        {
            var vonageWebhook = _mapper.Map<VonageWebhook>(m);
            vonageWebhook.DateTime = DateTime.UtcNow;
            _vonageWebhookRepository.Create(vonageWebhook);
            await _vonageWebhookRepository.SaveChangesAsync();

            await _hubContext.IncomingVoiceEvent(new WebhookVoiceModel
            {
                Sid = vonageWebhook.Uuid.ToString(), To = vonageWebhook.To,
                From = vonageWebhook.From, Status = vonageWebhook.Status,
                Name = ShareConstants.Vonage
            });

            return Ok(m);
        }
    }
}
