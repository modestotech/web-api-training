using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiTestProject1.Handlers
{
    public class MethodOverrideHandler : DelegatingHandler
    {
        readonly string[] _validMethods = {
            HttpMethod.Put.Method,
            HttpMethod.Delete.Method,
            HttpMethod.Head.Method,
            "VIEW", "PATCH"
        };

        const string _header = "X-HTTP-Method-Override";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Step 1:
            // Global message-level logic that muse be executed BEFORE the request is passed to the action method

            // X-HTTP-Method-Override header requires POST verb
            if (request.Method.Equals(HttpMethod.Post) && request.Headers.Contains(_header))
            {
                // Check for a valid new method from the list
                var method = request.Headers.GetValues(_header).FirstOrDefault();
                if (_validMethods.Contains(method, StringComparer.InvariantCultureIgnoreCase))
                {
                    // Change the request method to the override one
                    request.Method = new HttpMethod(method.ToUpperInvariant());
                }
            }

            // Collapse steps 2-4 since this handler doesn't do any response processing.
            return base.SendAsync(request, cancellationToken);
        }
    }
}