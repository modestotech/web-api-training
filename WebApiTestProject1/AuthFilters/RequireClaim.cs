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

namespace WebApiTestProject1.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireClaimAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// List of supported token schemes
        /// </summary>
        public readonly string[] ClaimTypes = null;

        /// <summary>
        /// If ture, full list of missing claims is included in the error response
        /// </summary>
        public bool IncludeMissingInResponse { get; set; }

        /// <summary>
        /// Constructor taking a comma-delimited list of claim types
        /// </summary>
        /// <param name="sList">Ex. [RequireClaim("urn:Issuer,urn:MyCustomClaim")]</param>
        public RequireClaimAttribute(string sList)
        {
            if (sList != null)
            {
                ClaimTypes = sList.Split(',')
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToArray();
            }
        }

        /// <summary>
        /// Constructor taking a param list of claim types
        /// </summary>
        /// <param name="list">Ex. [RequireClaim("urn:Issuer" ,"urn:MyCustomClaim", ClaimTypes.Email)]</param>
        public RequireClaimAttribute(params string[] list)
        {
            ClaimTypes = list.Where(a => !string.IsNullOrEmpty(a))
                .ToArray();
        }
        

        public override Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // First, ensure an IPrincipal was set, that is has an IIdentity, and that the
            // identity was authenticated; repeats some of Authorize attribute but
            // necessary check for this attribute as well. Likely no token was present at all.
            if (actionContext.RequestContext.Principal == null ||
                actionContext.RequestContext.Principal.Identity == null ||
                !actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                // In this specific case, probably no token pressent, we want the authentication
                // challenge to fire, so return 401 Unauthorized. I.e. basic issue is lack of 
                // authentication, not authenticated but disallowed.
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Unauthorized"
                };
                return Task.FromResult(0);
            }

            // User was authenticated, very the IIdentity is a ClaimsIdentity or some derived variant
            if (!(actionContext.RequestContext.Principal.Identity is ClaimsIdentity))
            {
                // Here we can return a 403 Forbidden, since the issue really is an authorization problem
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "Incorrect token for this operation"
                };
                return Task.FromResult(0);
            }

            // If no ClaimType list given, behave similar to Authorize -- just require any ClaimsIdentity
            // whech the above test has already done
            if (ClaimTypes == null || ClaimTypes.Length == 0)
            {
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }

            // Otherwise, search the identity's claims list for the full list of claims I need
            var claimsId = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            var missing = new List<string>();

            foreach (var c in ClaimTypes)
            {
                if (!claimsId.HasClaim(a => a.Type.Equals(c, StringComparison.InvariantCultureIgnoreCase)))
                {
                    missing.Add(c);
                }
            }

            if (missing.Count > 0)
            {
                // Return a 403 Forbidden since the issue really is an authorization problem,
                // get content-negotiated list if IncludeMissingInResponse

                if (IncludeMissingInResponse)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, missing);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                actionContext.Response.ReasonPhrase = "Identity lacks required claims";
                return Task.FromResult(0);
            }

            return base.OnAuthorizationAsync(actionContext, cancellationToken);        
        }
    }
}