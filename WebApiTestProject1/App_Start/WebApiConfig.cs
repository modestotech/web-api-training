using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using WebApiTestProject1.CustomConstraints;
using WebApiTestProject1.Handlers;

namespace WebApiTestProject1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Register delegating handlers
            config.MessageHandlers.Add(new FullPipelineTimerHandler());

            // Register contraint resolvers
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("validCustomerId", typeof(CustomerIdConstraint));

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);
        }
    }
}
