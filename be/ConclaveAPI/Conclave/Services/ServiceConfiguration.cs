using Conclave.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Conclave.Services
{
    /*
     * DI service configuration
     */
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Globally prefix all routes with specified attribute template
        /// </summary>
        public static void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
        }

        /// <summary>
        /// Globally prefix all routes with specified string
        /// </summary>
        public static void UseGeneralRoutePrefix(this MvcOptions opts, string prefix)
        {
            opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
        }

    }
}