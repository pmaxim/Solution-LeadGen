using Blazor.Extensions.Logging;
using BlazorWeb.Client;
using BlazorWeb.Client.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorWeb.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWeb.ServerAPI"));

builder.Services.AddApiAuthorization()
    .AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

StartUp.ConfigureDi(builder);
builder.Services.AddAutoMapper(typeof(Program));

//https://github.com/BlazorExtensions/Logging
// Add Blazor.Extensions.Logging.BrowserConsoleLogger
builder.Services.AddLogging(log => log
    .AddBrowserConsole()
    .SetMinimumLevel(LogLevel.Trace)
);

// Register a preconfigure SignalR hub connection.
// Note the connection isnt yet started, this will be done as part of the App.razor component
// to avoid blocking the application startup in case the connection cannot be established
builder.Services.AddSingleton<HubConnection>(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/hubs"))
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();
