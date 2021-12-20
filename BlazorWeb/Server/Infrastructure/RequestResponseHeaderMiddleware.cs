using BlazorWeb.Server.Models;
using Domain.Entities;
using Domain.Repositories;
using Newtonsoft.Json;

namespace BlazorWeb.Server.Infrastructure
{
    //https://www.c-sharpcorner.com/article/save-request-and-response-headers-in-asp-net-5-core2/
    public class RequestResponseHeadersMiddleware
    {
        private readonly RequestDelegate _next;


        public RequestResponseHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ISettingRepository settingRepository)
        {
            var model = new RequestResponseHeader();

            var requestHeaders = httpContext.Request.Headers
                .Where(x => model.RequestHeaders
                    .All(h => h != x.Key)).Select(x => x.Key);
            model.RequestHeaders.AddRange(requestHeaders);

            var uniqueResponseHeaders = httpContext.Response.Headers
                .Where(x => model.ResponseHeaders
                .All(h => h != x.Key)).Select(x => x.Key);
            model.ResponseHeaders.AddRange(uniqueResponseHeaders);

            settingRepository.Create(new Setting
            {
                Name = "RequestResponseHeadersMiddleware",
                Value = JsonConvert.SerializeObject(model)
            });

            await settingRepository.SaveChangesAsync();

            await _next.Invoke(httpContext);
        }
    }
}
