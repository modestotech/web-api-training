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
    public class FullPipelineTimerHandler : DelegatingHandler
    {
        const string _header = "X-API-Timer";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Step 1:
            // Global message-level logic that muse be executed BEFORE the request is passed to the action method
            var timer = Stopwatch.StartNew();

            // Step 2:
            // Call the rest of the pipeline, all the way to the response message
            var response = await base.SendAsync(request, cancellationToken);

            // Step 3:
            // Global message-level that must be executed AFTER the request has executed, before the final HTTP response message
            var elapsed = timer.ElapsedMilliseconds;
            Trace.WriteLine(" -- Pipeline+Action time msec: " + elapsed);
            response.Headers.Add(_header, elapsed + " msec");

            // Step 4:
            // Return the final HTTP response
            return response;
        }
    }
}