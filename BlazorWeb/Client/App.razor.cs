using BlazorWeb.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace BlazorWeb.Client
{
    public partial class App
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly AccountModel _account = new AccountModel();
        protected override async Task OnInitializedAsync()
        {
            _logger.LogDebug("OnInitialized");
            // launch the signalR connection in the background.
            // Note we dont want to block the rendering of the app until the connection is established
            // nor we want an exception to prevent the entire app from starting
            // Therefore we run it in the background without "await"
            // See: https://docs.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-5.0#handle-errors

            await ConnectWithRetryAsync(_cts.Token);
            // Once initialized the retry logic configured in the HubConnection will automatically attempt to reconnect
            // However, once it reaches its maximum number of attempts, it will give up and needs to be manually started again
            // handling this event will achieve that
            // See: https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-5.0&tabs=visual-studio#handle-lost-connection
            _hubConnection.Reconnected += error => Refresh(_cts.Token);
            _hubConnection.Closed += error => Refresh(_cts.Token);
        }

        private async Task<bool> ConnectWithRetryAsync(CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    _hubConnection.On<HangfireRingModel>("HangfireRunRing", HangfireRunRingChange);
                    _hubConnection.On<string>("EducationThread", EducationThreadChange);
                    _hubConnection.On<string>("EducationTask", EducationTaskChange);
                    await _hubConnection.StartAsync(token);
                    //await _hubConnection.InvokeAsync("addToGroup", _account, cancellationToken: token);
                    HubConnectChange(true);
                    _logger.LogDebug($"Hub start");
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    _logger.LogDebug("Hub CancellationRequested");
                    HubConnectChange(false);
                    return false;
                }
                catch (Exception ex)
                {
                    // Failed to connect, trying again in 5000 ms.
                    // This could be a random or incremental interval, similar to the reconnection strategy of the hub itself
                    // (Note the HubConnection retry strategy does not apply during initial connection, nor once it reaches its max number of attempts)
                    // See https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-5.0&tabs=visual-studio#handle-lost-connection
                    _logger.LogDebug($"Hub Error: {ex.Message}");
                    HubConnectChange(false);
                    await Task.Delay(5000, token);
                }
            }
        }

        private async Task<bool> Refresh(CancellationToken token)
        {
            //await _hubConnection.InvokeAsync("removeFromGroup", _account, cancellationToken: token);
            return await ConnectWithRetryAsync(token);
        }

        public async Task RemoveFromGroup()
        {
            await _hubConnection.InvokeAsync("removeFromGroup", _account);
        }

        public async Task<List<SignalRConnectionModel>> GetCurrentConnections()
        {
            return await _hubConnection.InvokeAsync<List<SignalRConnectionModel>>("getAllClient");
        }

        public bool ConnectedState()
        {
            return _hubConnection.State == HubConnectionState.Connected;
        }

        public async Task Connect()
        {
            await ConnectWithRetryAsync(new CancellationToken());
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogDebug($"Hub Dispose");
            HubConnectChange(false);
            _cts.Cancel();
            _cts.Dispose();
            await _hubConnection.DisposeAsync();
        }

        public async Task<ClaimsPrincipal> GetUser()
        {
            var authState = await _getAuthenticationStateAsync.GetAuthenticationStateAsync();
            return authState.User;
        }

        public delegate void ActionHangfireRunRing(HangfireRingModel data);
        public event ActionHangfireRunRing? HangfireRunRing;
        private void HangfireRunRingChange(HangfireRingModel data)
        {
            HangfireRunRing?.Invoke(data);
        }

        //Education
        public delegate void ActionEducationThread(string data);
        public event ActionEducationThread? EducationThread;
        private void EducationThreadChange(string data)
        {
            EducationThread?.Invoke(data);
        }

        public delegate void ActionEducationTask(string data);
        public event ActionEducationTask? EducationTask;
        private void EducationTaskChange(string data)
        {
            EducationTask?.Invoke(data);
        }

        //Education
        //Listener ButtonLeadPhonesUploadCsv-->PanelBottom
        public delegate void ActionPanelUpload(BoolString data);
        public event ActionPanelUpload? PanelUploadEvent;
        public void PanelUploadChange(BoolString data)
        {
            PanelUploadEvent?.Invoke(data);
        }

        //Listener UploadButtons-->TableUploadPhones
        public delegate void ActionTableUploadPhones(bool data);
        public event ActionTableUploadPhones? TableUploadPhonesEvent;
        public void TableUploadPhonesChange(bool data)
        {
            TableUploadPhonesEvent?.Invoke(data);
        }

        //Listener TableAccount-->TablesAccountsPhones
        public delegate void ActionTablesAccountsPhones(bool data);
        public event ActionTablesAccountsPhones? TablesAccountsPhonesEvent;
        public void TablesAccountsPhonesChange(bool data)
        {
            TablesAccountsPhonesEvent?.Invoke(data);
        }

        //Listener ButtonAccountEdit
        public delegate void ActionButtonAccountEdit(bool data);
        public event ActionButtonAccountEdit? ButtonAccountEditEvent;
        public void ButtonAccountEditChange(bool data)
        {
            ButtonAccountEditEvent?.Invoke(data);
        }

        //Listener ModalAccountEdit
        public delegate void ActionModalAccountEdit(int accountId);
        public event ActionModalAccountEdit? ModalAccountEditEvent;
        public void ModalAccountEditChange(int accountId)
        {
            ModalAccountEditEvent?.Invoke(accountId);
        }

        //Listener HubConnect
        public delegate void ActionHubConnect(bool isConnected);
        public event ActionHubConnect? HubConnectEvent;
        public void HubConnectChange(bool isConnected)
        {
            HubConnectEvent?.Invoke(isConnected);
        }
    }
}