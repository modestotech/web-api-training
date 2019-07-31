using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiTestProject1.Handlers;

namespace WebApiTestProject1.Filters
{
    /// <summary>
    /// Called when authorization must be checked
    /// </summary>
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // Get the base URL that the client actually used, taking load balancers into account
            var uri = actionContext.Request.GetSelfReferenceBaseUrl();

            if (uri == null || !uri.Scheme.ToLowerInvariant().Equals("https"))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "HTTPS Required"
                };
                return Task.FromResult(0); // Exits out the entire pipeline
            }

            return base.OnAuthorizationAsync(actionContext, cancellationToken); // Continues the pipeline
        }
    }
}