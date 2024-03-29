﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Filters
{
    // TODO: Decide if your filter should allow multiple instances per controller
    // or per-method; set AllowMultiple to true if so
    public class ActionFilterTemplateAttribute : ActionFilterAttribute
    {

        // TODO: If in need of constructor arguments, create properties to hold them and public
        // constructors that accept them.
        public ActionFilterTemplateAttribute()
        {
             
        }

        // Executed BEFORE the controller action method is called
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // STEP 1:
            // Any logic you want to do BEFORE the rest of the action filter chain is called, and BEFORE the action method itself.

            // STEP 2:
            // Call the rest of the action filter chain
            await base.OnActionExecutingAsync(actionContext, cancellationToken);
            
            // STEP 3:
            // Logic AFTER the other action filters, but BEFORE the action method itself.
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            // STEP 1:
            // Any logic you want to do BEFORE the rest of the action filter chain is called, and AFTER the action method itself.

            // STEP 2:
            // Call the rest of the action filter chain
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            // STEP 3:
            // Logic AFTER the other action filters, and AFTER the action method itself.
        }
    }
}