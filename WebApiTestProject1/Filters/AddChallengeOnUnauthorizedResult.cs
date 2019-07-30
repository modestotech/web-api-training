using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiTestProject1.Filters
{
    /// <summary>
    /// Generic WWW-challenge IHttpActionResult implementation
    /// </summary>
    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AuthenticationHeaderValue Challenge { get; private set; }
        public IHttpActionResult InnerResult { get; private set; }

        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue authenticationHeaderValue, IHttpActionResult innerResult)
        {
            Challenge = authenticationHeaderValue;
            InnerResult = innerResult;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) 
        {
            HttpResponseMessage response = await InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Only add one challenge per auth scheme
                if (!response.Headers.WwwAuthenticate.Any((h) => h.Scheme == Challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(Challenge);
                }
            }

            return response;
        }
    }
}