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
    /// <summary>
    /// Authorization happens AFTER authentication. By the time your authorization
    /// filter is called, the authenicated identity should be set (if credentails were provided).
    /// </summary>
    // TODO:    Decide if your filter should allow multiple instances per controller or
    //          per-method; set AllowMultiple to false if not.
    public class AuthorizationFilterTemplateAttribute : AuthorizationFilterAttribute
    {
        // TODO:    If you need constructor arguments, create properties to hold them
        //          and public constructors that accept them.
        public AuthorizationFilterTemplateAttribute()
        { }

        public override async Task OnAuthorizationAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            // STEP1:   Perform your authorizaton logic
            // The authentication filters should have set an IPrincipal for your
            // with various properties
            var principal = actionContext.RequestContext.Principal;

            // ...though it's possible to have an authorization filter withour or
            // independent of authentication; perhaps based the presence of certain
            // http headers in the request. In that case use the appropriate logic.

            // You can cast the IPrincipal to a specific class type to access the
            // claims or priperties of the authenticated principal:
            //var specificIdentityType = principal.Identity as ClaimsIdentity;
            //var claim = specificIdentityType.Claims.FirstOrDefault(a => a.Type.Equals("MyClaim"));

            var authorized = true; // Could also be async authorizaton logic

            // STEP2:   If authorization fails, set the HTTP response and exit
            if (!authorized)
            {
                // Which code to return is by default 403, but some prefer 404
                // to not reveal to hackers that they were blocked by an authorization
                // issue.
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                ; //{ Content = ...};
                return;
            }

            await base.OnAuthorizationAsync(actionContext, cancellationToken);        
        }
    }
}