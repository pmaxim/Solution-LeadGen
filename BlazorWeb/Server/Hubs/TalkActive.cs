using BlazorWeb.Shared.Models;
using Domain.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWeb.Server.Hubs
{
    //https://docs.microsoft.com/en-us/aspnet/signalr/overview/advanced/dependency-injection
    //https://github.com/DaniJG/blazor-surveys.git

    public interface ITalkActive
    {
        List<SignalRConnectionModel> GetAllClient();
        Task IncomingVoiceEvent(WebhookVoiceModel incomingVoiceEvent);
    }

    public class TalkActive : Hub, ITalkActive
    {
        private readonly ISettingRepository _repoSetting;
        private static readonly List<SignalRConnectionModel> Connections = new List<SignalRConnectionModel>();

        public TalkActive(ISettingRepository repoSetting)
        {
            _repoSetting = repoSetting;
        }

        public override async Task OnConnectedAsync()
        {
            var userName = GetUserName();
            if (!string.IsNullOrEmpty(userName))
            {
                Connections.Add(new SignalRConnectionModel
                {
                    UserName = userName,
                    ConnectionId = Context.ConnectionId,
                    Ip = GetUserIp()
                });

                await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            }
           
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var current = Connections.SingleOrDefault(z => z.ConnectionId == Context.ConnectionId);
            if (current != null)
            {
                Connections.Remove(current);
            }
            var userName = GetUserName();
            if (!string.IsNullOrEmpty(userName)) await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);

            await base.OnDisconnectedAsync(exception);
        }

        public List<SignalRConnectionModel> GetAllClient()
        {
            return Connections;
        }

        public async Task IncomingVoiceEvent(WebhookVoiceModel incomingVoiceEvent)
        {
            await Clients.All.SendAsync("IncomingVoiceEvent", incomingVoiceEvent);
        }

        private string GetUserIp()
        {
            var feature = Context.Features.Get<IHttpConnectionFeature>();
            return feature!.RemoteIpAddress!.ToString();
        }

        private string GetUserName()
        {            
            return Context.User?.Identity?.Name!;
        }
    }
}
