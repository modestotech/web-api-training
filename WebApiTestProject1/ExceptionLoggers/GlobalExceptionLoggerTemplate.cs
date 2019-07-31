using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace WebApiTestProject1.ExceptionLoggers
{
    /// <summary>
    /// Global unhandled excpetion logging/analytics template. 
    /// Can be several loggers in the app. Good to have one logger for each requirement and responsibility.
    /// </summary>
    /// <remarks>
    /// To register one or more loggers:
    /// <code>
    /// config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLoggerTemplate());
    /// </code>
    /// </remarks>
    public class GlobalExceptionLoggerTemplate : ExceptionLogger
    {
        /// <summary>
        /// Required ExceptionLogger method to process an exception
        /// </summary>
        /// <remarks>
        /// Important, not every ExceptionLoggerContext field will be set depending on where the
        /// exception occurs, but you can minimally count on the Exception and Request properties
        /// </remarks>
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            // STEP 1:  Do whatever analytics you like on the exception
            var ex = context.Exception;

            // Example - simple trace logging
            //Trace.WriteLine("*** Exception: " + ex.ToString());

            return Task.FromResult(0);
        }
    }
}