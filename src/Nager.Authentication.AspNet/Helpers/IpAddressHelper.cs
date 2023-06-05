using Microsoft.AspNetCore.Http;

namespace Nager.Authentication.AspNet.Helpers
{
    public static class IpAddressHelper
    {
        public static string? GetIpAddress(this HttpContext httpContext)
        {
            if (httpContext.Request != null)
            {
                if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
                {
                    return httpContext.Request.Headers["X-Real-IP"];
                }

                if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return httpContext.Request.Headers["X-Forwarded-For"];
                }
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
