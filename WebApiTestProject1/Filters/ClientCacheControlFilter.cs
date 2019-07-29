using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    // Note from teacher: Also short cache time spans can be useful, even 10 seconds
    public enum ClientCacheControl
    {
        Public, // Can be cached by intermediate devices even if authenthication was used
        Private, // Browser-only, no intermediate chacing, typically for per-user data
        NoCache // No caching by browser or intermediate devices
    };

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ClientCacheControlFilterAttribute : ActionFilterAttribute
    {
        public ClientCacheControl CacheType;
        public double CacheSeconds;

        public ClientCacheControlFilterAttribute(double seconds = 60.0)
        {
            CacheType = ClientCacheControl.Private;
            CacheSeconds = seconds;
        }

        public ClientCacheControlFilterAttribute(ClientCacheControl cacheType, double seconds = 60.0)
        {
            CacheType = cacheType;
            CacheSeconds = seconds;

            if (cacheType == ClientCacheControl.NoCache)
                CacheSeconds = -1;
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            // Call the rest of the pipeline
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            // Filters and action method itself has reached the end, now add the headers
            if (CacheType == ClientCacheControl.NoCache)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoStore = true
                };

                // For older browsers
                actionExecutedContext.Response.Headers.Pragma.TryParseAdd("no-cache");

                // Create a date, if not present, so we can match the dates so that it immediately expires
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                actionExecutedContext.Response.Content.Headers.Expires = actionExecutedContext.Response.Headers.Date;
            }
            else
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = (CacheType == ClientCacheControl.Public),
                    Private = (CacheType == ClientCacheControl.Private),
                    NoCache = false,
                    MaxAge = TimeSpan.FromSeconds(CacheSeconds)
                };

                // Create a date, if not present, so we can match the dates so that it can be set
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                actionExecutedContext.Response.Content.Headers.Expires = actionExecutedContext.Response.Headers.Date.Value.AddSeconds(CacheSeconds);
            }
        }
    }
}