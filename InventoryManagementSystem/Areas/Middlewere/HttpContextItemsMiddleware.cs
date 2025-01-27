using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
namespace SessionSample
{
    #region snippet1
    public class HttpContextItemsMiddleware
    {
        private readonly RequestDelegate _next;
        public static readonly object HttpContextItemsMiddlewareKey = new Object();

        public HttpContextItemsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Items[HttpContextItemsMiddlewareKey] = "K-9";

            await _next(httpContext);
        }
    }

    public static class HttpContextItemsMiddlewareExtensions
    {
        public static IApplicationBuilder
            UseHttpContextItemsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpContextItemsMiddleware>();
        }
    }
    #endregion
}
