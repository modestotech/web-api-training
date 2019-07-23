using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using WebApiTestProject1.CustomConstraints;

namespace WebApiTestProject1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("validCustomerId", typeof(CustomerIdConstraint));
            config.MapHttpAttributeRoutes(constraintResolver);
        }
    }
}
