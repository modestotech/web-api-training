using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiTestProject1.ExceptionFilters
{
    public class NotImplementedExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var ex = actionExecutedContext.Exception;

            // Pick up NotImplementedException
            if (ex is NotImplementedException)
            {
                // Put a status code that is useful for the user
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

           await Task.FromResult(0);
        }
    }
}