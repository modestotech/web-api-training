﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApiTestProject1.ExceptionFilters
{
    /// <summary>
    /// Web API Exception filter template. Generally meant for action- or controller-specific
    /// exceptions where you want common processing to occur.
    /// </summary>
    public class ExceptionFilterTemplate : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {

            // STEP 1:  Do any internal processing you want, for example
            //          trace logging, metrics gathering, etc. as a result 
            //          of getting this exception. Do it async if it involves
            //          expensive operations, like I/O.

            var ex = actionExecutedContext.Exception;
            //......

            // ...AND/OR...

            // STEP 2:  Create a custom HttpResponseMessage to return instead
            //          of the default response the Web API engine will send as
            //          a result of this exception.
           //actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
           //{
           //    Content = ...etc.
           //}

           await Task.FromResult(0);
        }
    }
}