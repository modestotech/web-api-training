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
    public class RemoveBadHeadersHandler : DelegatingHandler
    {
        readonly string[] _badHeaders = { "X-Powered-By", "X-AspNet-Version", "Server" };

        // This handler doesn't work as intended as these specific headers can't be removed in this way as they have to be removed in Web.config
        // This example shows that some things can't be achieved in the pipeline as they are part of the overlaying ASP.NET or IIS layer
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Step 2:
            // Call the rest of the pipeline, all the way to the response message
            var response = await base.SendAsync(request, cancellationToken);

            // Step 3:
            // Global message-level that must be executed AFTER the request has executed, before the final HTTP response message
            // Remove bad headers in a for loop
            foreach (var header in _badHeaders)
            {
                response.Headers.Remove(header);
            }

            // Step 4:
            // Return the final HTTP response
            return response;
        }
    }

}