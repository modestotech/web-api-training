using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiTestProject1.ExceptionFilters
{
    public class InvalidAccountIdExceptionFilter : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var ex = actionExecutedContext.Exception;

            // Pick up NotImplementedException
            if (ex is ArgumentOutOfRangeException)
            {
                // Put a status code that is useful for the user
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "InvalidAccountId",
                    Content = new StringContent("Account Ids must be in the range 1 to 50")
                };
            }

           await Task.FromResult(0);
        }
    }
}