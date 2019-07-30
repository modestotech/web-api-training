using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiTestProject1.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        // True if the bound FromBody parameter is required
        public bool BodyRequired { get; set; }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // STEP 1:
            // Any logic you want to do BEFORE the rest of the action filter chain is called, and BEFORE the action method itself.

            // Note that null is valid from the perspective of the ModelState
            // i.e. passing in no body payload will return 200
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
            else if (BodyRequired) 
            {
                // Get definition of parameter bindings
                // Note that any checks could be done here, for example check that parameters
                // are consistent between each other, including checks between URL and FromBody parameters
                foreach (var b in actionContext.ActionDescriptor.ActionBinding.ParameterBindings)
                {
                    // Find one name FromBody
                    if (b.WillReadBody)
                    {
                        // One FromBody is found
                        if (!actionContext.ActionArguments.ContainsKey(b.Descriptor.ParameterName)
                            || actionContext.ActionArguments[b.Descriptor.ParameterName] == null)
                        {
                            // If the bound FromBody parameter is null
                            actionContext.Response = actionContext.Request.CreateErrorResponse(
                                HttpStatusCode.BadRequest, b.Descriptor.ParameterName + " is required");
                        }
                        // Since only one FromBody can exist, let's abort
                        break;
                    }
                }
            }

            // STEP 2:
            // Call the rest of the action filter chain
            await base.OnActionExecutingAsync(actionContext, cancellationToken);

            // STEP 3:
            // Logic AFTER the other action filters, but BEFORE the action method itself.
        }
    }
}