using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WebApiTestProject1.ExceptionHandlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            var ex = context.Exception;

            // in this example, we simply strip off the stack trace and leave only the error message
            var responseMsg = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            context.Result = new ResponseMessageResult(responseMsg);

            return base.HandleAsync(context, cancellationToken);
        }
    }
}
