using AutoMapper;
using Blazored.LocalStorage;
using BlazorWeb.Shared.Services;
using Darnton.Blazor.DeviceInterop.Geolocation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWeb.Client.Infrastructure
{
    public static class StartUp
    {
        public static void ConfigureDi(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<MyScopeService>();

            //builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddTransient(sp => new HttpClient
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                })
                .AddBlazoredLocalStorage();

            builder.Services.AddScoped<IGeolocationService, GeolocationService>();
        }

        private static void ConfigureAutoMapper(WebAssemblyHostBuilder builder)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperClientProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
        }
    }
}
