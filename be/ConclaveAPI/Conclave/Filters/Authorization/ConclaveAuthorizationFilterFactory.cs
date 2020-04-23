using Conclave.Services;
using Conclave.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Conclave.Filters.Authorization
{
    public class ConclaveAuthorizationFilterFactory : IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // manually find and inject necessary dependencies.
            var context = (RedisClient)serviceProvider.GetService(typeof(RedisClient));
            return new ConclaveAuthorizationFilter(context);
        }
    }
}
