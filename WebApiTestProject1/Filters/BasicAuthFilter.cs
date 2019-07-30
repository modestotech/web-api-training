using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    public class BasicAuthFilter : Attribute, IAuthenticationFilter
    {

        public const string SupportedTokenScheme = "Basic";
        public bool AllowMultiple { get { return false; } }
        public bool SendChallenge { get; set; }

        public BasicAuthFilter()
        {
            SendChallenge = true;
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context,
            CancellationToken cancellationToken)
        {
            var authHeader = context.Request.Headers.Authorization;

            if (authHeader == null)
                return;

            var tokenType = authHeader.Scheme;
            if (!tokenType.Equals(SupportedTokenScheme))
                return;

            var credentials = authHeader.Parameter;
            if (string.IsNullOrEmpty(credentials))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", context.Request);
            }

            IPrincipal principal = await ValidateCredentialsAsync(credentials, cancellationToken);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", context.Request);
            }
            else
            {
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
            // Credential validation logic here
            var subject = ParseBasicAuthCredential(credentials);

            // TODO: Create an IPrincipal (generic or custom) holding an IIdentity. 
            if (string.IsNullOrEmpty(subject.Item2) || subject.Item2 != "abc123") // Very easy dummt validation
                return null;

            IList<Claim> claimsCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, subject.Item1),
                // Other standard or custom claims can be added here, based on username/password
                // Get all user data needed from a database
                new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("o")),
                new Claim("urn:MyCustomClaim", "my special value")
            };

            // We'll include the specific token scheme as "authentication type" that was 
            // successful in authenticating the user so downstream code can verify it was
            // a token type sufficient for the activity the code is attempting.
            var identity = new ClaimsIdentity(claimsCollection, SupportedTokenScheme);
            var principal = new ClaimsPrincipal(identity);

            return await Task.FromResult(principal);
        }

        private Tuple<string, string> ParseBasicAuthCredential(string credential)
        {
            string password = null;

            var subject = (Encoding.GetEncoding("iso-8859-1")
                .GetString(Convert.FromBase64String(credential)));

            if (string.IsNullOrEmpty(subject))
                return new Tuple<string, string>(null, null);

            if (subject.Contains(":"))
            {
                var index = subject.IndexOf(':');
                password = subject.Substring(index + 1);
                subject = subject.Substring(0, index);
            }

            return new Tuple<string, string>(subject, password);
        }
    }
}