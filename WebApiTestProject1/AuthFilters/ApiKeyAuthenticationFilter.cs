using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    public class ApiKeyAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public const string ApiKeyHeader = "X-API-Key";
        public const string SupportedTokenScheme = "ApiKey";
        public bool AllowMultiple { get { return false; } }
        public bool SendChallenge { get; set; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
        {
            string apiKey = null;

            // STEP 1: extract your credentials from the request.  Generally this should be the 
            //         Authorization header, which the rest of this template assumes, but
            //         could come from any part of the request headers.
            // try the legacy header first
            if (context.Request.Headers.Contains(ApiKeyHeader))
                apiKey = context.Request.Headers.GetValues(ApiKeyHeader).FirstOrDefault();
            if (string.IsNullOrEmpty(apiKey))
            {
                var authHeader = context.Request.Headers.Authorization;
                // if there are no credentials, abort out
                if (authHeader == null)
                    return;
                // STEP 2: if the token scheme isn't understood by this authenticator, abort out
                var tokenType = authHeader.Scheme;
                if (!tokenType.Equals(SupportedTokenScheme))
                    return;
                // STEP 3: Given a valid token scheme, verify credentials are present
                apiKey = authHeader.Parameter;
                if (string.IsNullOrEmpty(apiKey))
                {
                    // no credentials sent with the scheme, abort out of the pipeline with an error result
                    context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
                    return;
                }
            }
            // STEP 4: validate the credentials.  Return an error if invalid, else set the IPrincipal 
            //         on the context.
            IPrincipal principal = await ValidateCredentialsAsync(apiKey, cancellationToken);
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", context.Request);
            }
            else
            {
                // We have a valid, authenticated user; save off the IPrincipal instance
                context.Principal = principal;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (SendChallenge)
            {
                context.Result = new AddChallengeOnUnauthorizedResult(
                    new AuthenticationHeaderValue(SupportedTokenScheme),
                    context.Result);
            }

            return Task.FromResult(0);
        }

        private async Task<IPrincipal> ValidateCredentialsAsync(string credentials, CancellationToken cancellationToken)
        {
            // validate the 8 char length requirement
            if (!Helpers.Validation.IsValidApiKey(credentials))
                return null;

            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials),
                new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("o")),
                new Claim("urn:ClientAccount", credentials.Substring(0,3))
            };

            var identity = new ClaimsIdentity(claimCollection, SupportedTokenScheme);
            var principal = new ClaimsPrincipal(identity);
            return await Task.FromResult(principal);
        }
    }
}