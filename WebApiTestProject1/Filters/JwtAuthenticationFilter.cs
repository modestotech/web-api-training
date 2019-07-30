using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    public class JwtAuthenticationFilter : Attribute, IAuthenticationFilter
    {

        public const string SupportedTokenScheme = "Bearer";
        public bool AllowMultiple { get { return false; } }

        string _audience = "https://my.company.com";
        string _validIssuer = "http://my.tokenissuer.com";

        private readonly X509SecurityKey _signingCredentials;

        public bool SendChallenge { get; set; }

        public JwtAuthenticationFilter()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/CourseCert.cer");
            var certificate = new X509Certificate2(filePath);
            _signingCredentials = new X509SecurityKey(certificate);
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
                context.ErrorResult = new AuthentiationFailureResult("Missing credentials", context.Request);
                return;
            }

            try
            {
                IPrincipal principal = await ValidateCredentialsAsync(credentials, cancellationToken);

                if (principal == null)
                {
                    context.ErrorResult = new AuthentiationFailureResult("Invalid security token", context.Request);
                }
                else
                {
                    context.Principal = principal;
                }
            }
            catch (Exception stex) when (stex is SecurityTokenInvalidLifetimeException ||
                                            stex is SecurityTokenExpiredException ||
                                            stex is SecurityTokenNoExpirationException ||
                                            stex is SecurityTokenNotYetValidException)
            {
                context.ErrorResult = new AuthentiationFailureResult("Security token lifetime error", context.Request);
            }
            catch (Exception)
            {
                context.ErrorResult = new AuthentiationFailureResult("Invalid security token", context.Request);
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
            var jwtHandler = new JwtSecurityTokenHandler();

            // Verify validity of token
            var isValidJwt = jwtHandler.CanReadToken(credentials);
            if (!isValidJwt)
                return null;

            // Time to validate the Jwt internals
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudiences = new[] { _audience },

                ValidateIssuer = true,
                ValidIssuers = new[] { _validIssuer },

                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = new[] { _signingCredentials },

                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5), // Limit the lifetime padding

                NameClaimType = ClaimTypes.NameIdentifier,
                AuthenticationType = SupportedTokenScheme,
            };

            SecurityToken validatedToken = new JwtSecurityToken();
            ClaimsPrincipal principal = jwtHandler.ValidateToken(credentials, validationParameters, out validatedToken);

            // Locally generated claims that should be available to downstream code
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:Issuer",
                validatedToken.Issuer));
            ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("urn:TokenScheme",
                SupportedTokenScheme));

            // If any downstream code might want the original token string, maybe to 
            // make downstream calls, store it in a standard claim name or the bootstrap
            // context.
            ((ClaimsIdentity)principal.Identity).BootstrapContext = credentials;

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