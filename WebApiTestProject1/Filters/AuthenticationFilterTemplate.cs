using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    /// <summary>
    /// Authentication filter template with attribute support so that it can be set per-controller or per-route
    /// Especially for web services combined into web app, this should be added to WebApiConfig.cs Register method
    /// </summary>
    /// <remarks>
    /// Authentication filter template with attribute support so that it can be set per-controller or per-route
    /// Especially for web services combined into web app, this should be added to WebApiConfig.cs Register method
    /// <code>
    ///      config.SuppressHostPrincipal();
    /// </code>
    /// This ensures that the IIS layer hasn't added some IPrincipal to the request based on the 
    /// application authentication, prior to your own web service authentication logic executing.
    /// There's no harm in always adding that line, even for standalone web services, just to be sure.
    /// It should always be included.
    /// </remarks>
    public class AuthenticationFilterTemplate : Attribute, IAuthenticationFilter
    {

        /// <summary>
        /// Set to the Authorization header Scheme value that this filter is intended to support
        /// </summary>
        public const string SupportedTokenScheme = "MyCustomToken";

        // TODO: Decide if your filter should allow multiple instances per controller or
        // per-method; set AllowMultiple to true if so
        public bool AllowMultiple { get { return false; } }

        /// <summary>
        /// True if the filter supports WWW-Authenticate challenge headers, defaults to false.
        /// </summary>
        /// <remarks>
        /// Scenarios not involving users from browsers using either the Basic or Digest
        /// authorization cheme, challenges are not inherently required, a machine cannot likely 
        /// process the challenge anyway, therefore the default value is false.
        /// </remarks>
        public bool SendChallenge { get; set; }

        /// <summary>
        /// Logic to athenticate the credentials, Must do one of:
        ///  -- abort out, do nothing, if it cant understand the token scheme presented.
        ///     - Note that abort doesn't equals error, as there may be other authentication filter that matches
        ///  -- set context.ErrorResult to an IHttpActionResult holding reason for invalid authentication.
        ///  -- set context.Principal to an IPrincipal if authenticated.
        /// </summary>        
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // STEP 1:  Extract your credentials from the request. Generally this should be the
            //          authorization header, which the rest of this template assumes, 
            //          but could come from any part of the request headers.
            var authHeader = context.Request.Headers.Authorization;

            // Note that abort doesn't equals error, as there may be other authentication filter that matches

            // If there are no credentials, abort out
            if (authHeader == null)
                return;

            // STEP 2: If the token scheme isn't understood by this authenticator, abort out.
            var tokenType = authHeader.Scheme;
            if (!tokenType.Equals(SupportedTokenScheme))
                return;

            // STEP 3: Given a valid token scheme, verify that credentials are present
            var credentials = authHeader.Parameter;
            if (string.IsNullOrEmpty(credentials))
            {
                // No credentials sent with the scheme, abort out of the pipeline with an error result
                context.ErrorResult = new AuthentiationFailureResult("Missing credentials", context.Request);
            }

            // STEP 4: Validate the credentials. Return an error if invalid, else set the IPrincipal on the context.
            IPrincipal principal = await ValidateCredentialsAsync(credentials, cancellationToken);

            if (principal == null)
            {
                context.ErrorResult = new AuthentiationFailureResult("Invalid credentials", context.Request);
            }
            else
            {
                // We have a valid, authenticated user, save off the IPrincipal instance
                context.Principal = principal;
            }
        }

        /// <summary>
        /// Extra logic associated with Basic/Digest auth scheme, to add the 
        /// WWW-Authenticate header; for other token scehmes, you can just do nothing
        /// as shown below
        /// </summary>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            // If this filter wants to support WWW-Authenticate header challenges, 
            // add one to the result
            if (SendChallenge)
            {
                context.Result = new AddChallengeOnUnauthorizedResult(
                    new AuthenticationHeaderValue(SupportedTokenScheme),
                    context.Result);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Internal method to validate the credentials included in the request,
        /// returning an IPrincipal for the resulting authenticated entity.
        /// </summary>
        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials, CancellationToken cancellationToken)
        {
            // TODO: Your credential validation logic here, hopefully async

            // TODO: Create an IPrincipal (generic or custom) holding an IIdentity (generic
            //       or custom). Note a very useful IPrincipal/IIdentity is 
            //       ClaimsPrincipal/ClaimsIdentity if you need both subjects identifier 
            //       (ex. user name), plus a set of attributes (claims) about the subject.
            var principal = new GenericPrincipal(new GenericIdentity(credentials), null);

            return await Task.FromResult(principal);
        }
    }
}