namespace BlazorWeb.Server.Infrastructure
{
    public static class RequestResponseHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseHeadersMiddleware>();
        }
    }
}
