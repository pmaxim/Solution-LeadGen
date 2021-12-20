using BrowserInterop;
using BrowserInterop.Extensions;

namespace BlazorWeb.Client.Shared
{
    public partial class NavMenu
    {
        private App? _main;
        private bool _collapseNavMenu = true;
        private bool _expandSettings = false;
        private string _link = string.Empty;
        private bool _full = true;
        private bool _signalRonLine = false;
        private bool _onLine = false;
        private WindowInterop? _window = null;
        private WindowNavigator? _navigator = null;
        private WindowNavigatorBattery? _battery;
        private bool? _javaEnabled;
        private IAsyncDisposable? _connectionChangeEventHandler;
        private bool _admin = false;
        private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;
        protected override void OnInitialized()
        {
            _main = (App)_mss["App"];
            _main.HubConnectEvent += HubConnectEvent;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var user = await _authenticationStateProvider.GetAuthenticationStateAsync();
                _admin = user.User.IsInRole("admin") || user.User.IsInRole("developer");
                _window = await _jsRuntime.Window();
                if (_window == null)
                    return;
                _navigator = await _window.Navigator();
                _onLine = _navigator.Online;
                Console($"_onLine = {_onLine}");
                _battery = await _navigator.GetBattery();
                _javaEnabled = await _navigator.JavaEnabled();
                if (_navigator.Connection != null)
                {
                    _connectionChangeEventHandler = await _navigator.Connection.OnChange(async () =>
                    {
                        _navigator = await _window.Navigator();
                        _onLine = _navigator.Online;
                        Console($"_onLine = {_onLine}");
                        if (!_onLine)
                            _signalRonLine = false;
                        await InvokeAsync(StateHasChanged);
                    });
                }
            }
        }

        private async void HubConnectEvent(bool isConnected)
        {
            _signalRonLine = isConnected;
            Console($"_signalRonLine = {_signalRonLine}");
            await InvokeAsync(StateHasChanged);
        }

        private void ToggleNavMenu(string link)
        {
            _collapseNavMenu = !_collapseNavMenu;
            _expandSettings = false;
            _link = link;
            _signalRonLine = _main!.ConnectedState();
        }

        private void MainOrChat(bool flag)
        {
            _full = flag;
        }

        public void ChatLoad()
        {
            _full = false;
            StateHasChanged();
        }

        public void GoLink()
        {
            _full = true;
            StateHasChanged();
            _navigation.NavigateTo(_link);
        }

        private async void Console(string text)
        {
            if (_window == null)
                return;
            await _window.Console.Log(text);
        }
    }
}