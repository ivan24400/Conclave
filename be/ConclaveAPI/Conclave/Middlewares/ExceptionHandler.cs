using Conclave.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Conclave.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                CLogger.Log(e);
                httpContext.Response.StatusCode = 500;
                var res = new { errorCode = "internal_error", errorDescription = "Unknown error has occurred" };
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(res));
            }
        }

    }
}
