using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiTestProject1.Handlers
{
    public class APIKeyHeaderHandler : DelegatingHandler
    {
        // Name of custom header to look for (user by Swagger)
        public const string _apiKeyHeader = "X-API-Key";
        // Name of api key query string key
        public const string _apiQueryString = "api_key";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Step 1:
            // Global message-level logic that muse be executed BEFORE the request is passed to the action method
            string apiKey = null;

            // This layer should not contain authorization (single responsibility)
            // therefore the commented pieces, below should not be here, but in the authorization filter

            /*
             *  // COMMENTED OUT AS IT SHOULD BE IN THE AUTHORIZATION LAYER
                // Swagger is itself a route, and should be accessible without an API key
                if (request.RequestUri.Segments[1].ToLowerInvariant().StartsWith("swagger"))
                    return base.SendAsync(request, cancellationToken);
             */
            if (request.Headers.Contains(_apiKeyHeader))
            {
                apiKey = request.Headers.GetValues(_apiKeyHeader).FirstOrDefault();
            }
            else
            {
                // Check if it's in the query string instead
                var queryString = request.GetQueryNameValuePairs();
                var keyValuePair = queryString.FirstOrDefault(a => a.Key.ToLowerInvariant().Equals(_apiQueryString));

                if (!string.IsNullOrEmpty(keyValuePair.Value))
                    apiKey = keyValuePair.Value;
            }

            /*
             *  // COMMENTED OUT AS IT SHOULD BE IN THE AUTHORIZATION LAYER
                // Was any api key present, if not, abort the request
                if (string.IsNullOrEmpty(apiKey))
                {
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Missing API key");

                    return Task.FromResult(response);
                };
             */

            // Save the value to properties
            request.Properties.Add(_apiKeyHeader, apiKey);

            // Compress steps 2, 3, and 4 into one line, sice we don't need any post-request processing
            return base.SendAsync(request, cancellationToken);
        }
    }

    // Exposes a nice, clean method, so that user's don't have to look up a specific header, key, or anything, 
    // invoked in GET products/
    public static class HttpRequestMessageApiKeyExtension
    {
        public static string GetApiKey(this HttpRequestMessage request)
        {
            if (request == null)
                return null;

            if (request.Properties.TryGetValue(APIKeyHeaderHandler._apiKeyHeader, out object apiKey))
                return (string)apiKey;

            return null;
        }
    }
}