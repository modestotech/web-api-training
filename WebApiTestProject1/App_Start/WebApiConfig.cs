using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using WebApiTestProject1.CustomConstraints;
using WebApiTestProject1.Filters;
using WebApiTestProject1.Handlers;

namespace WebApiTestProject1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // This flag makes the API prefer the controller instead of the directory
            System.Web.Routing.RouteTable.Routes.RouteExistingFiles = true;

            // Register delegating handlers
            /*
            config.MessageHandlers.Add(new RemoveBadHeadersHandler());
            config.MessageHandlers.Add(new APIKeyHeaderHandler());
            config.MessageHandlers.Add(new RemoveBadHeadersHandler());
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.MessageHandlers.Add(new ForwardedHeadersHandler());
            */

            // Register Authenthication, Authorization and Action filters (for those that should be active globally)
            // Usually preferrable to register them per controller, or even per route
            // config.Filters.Add(new ActionFilterTemplateAttribute());

            // Global registration, as AllowMultiple = true is set on the FilterAttribute, there can be multiple instances
            /*
            config.Filters.Add(new RouteTimerFilterAttribute("Global"));
            */

            // Register contraint resolvers
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("validCustomerId", typeof(CustomerIdConstraint));

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);
        }
    }
}
