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
    public class GlobalExceptionLogger : ExceptionLogger
    {
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            var ex = context.Exception;

            Trace.WriteLine("*** Exception: " + ex.ToString());

            return Task.FromResult(0);
        }
    }
}