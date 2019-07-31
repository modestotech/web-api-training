using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WebApiTestProject1.ExceptionHandlers
{
    /// <summary>
    /// Global Exception handler template
    /// </summary>
    /// <remarks>
    /// Don't forget to register in WebApiConfig.cs: (only one can be used)
    /// <code>
    /// config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
    /// </code>
    /// Note that you can daisy-chain several handler together using the "inner handler" model
    /// if you need more than the one Web Api allows you to register. 
    /// Just add a constructor taking the inner handler, and a property to hold it.
    /// </remarks>
    public class GlobalExceptionHandlerTemplate : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            // STEP 1:  Exit if we cannot handle the exception

            // Nothing we can do if the context is not present
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Verify this exception should be handled at all; exit if not
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            // STEP 2:  Create an IHttpActionResult from the exception as required
            var ex = context.Exception;

            // in this example, we simply strip off the stack trace and leave only the error message
            //var responseMsg = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            //context.Result = new ResponseMessageResult(responseMsg);

            return base.HandleAsync(context, cancellationToken);
        }
    }
}
