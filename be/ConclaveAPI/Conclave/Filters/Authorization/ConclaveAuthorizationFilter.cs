using Conclave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Conclave.Utils
{
    public class ConclaveAuthorizationFilter : IAuthorizationFilter
    {
        private readonly RedisClient _cache;
        public ConclaveAuthorizationFilter()
        {
        }
        public ConclaveAuthorizationFilter(RedisClient cache)
        {
            _cache = cache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionAttrList = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(true);
            var ctrlAttrList = (context.ActionDescriptor as ControllerActionDescriptor).ControllerTypeInfo.GetCustomAttributes(true);
            if (actionAttrList.OfType<AllowAnonymousAttribute>().Count() == 0 && ctrlAttrList.OfType<AllowAnonymousAttribute>().Count() == 0)
            {
                // Authorized access
                bool isAuthorized = true;
                try
                {
                    var accessToken = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                    if (!AuthToken.VerifyAccessToken(accessToken))
                    {
                        isAuthorized = false;
                    }
                }
                catch (Exception e)
                {
                    isAuthorized = false;
                }
                if (!isAuthorized)
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult(new { error = "unauthorized", errorDesc = "Unauthorized access" });
                }

            }

        }
    }
}
