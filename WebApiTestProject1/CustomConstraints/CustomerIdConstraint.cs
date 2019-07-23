using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace WebApiTestProject1.CustomConstraints
{
    public class CustomerIdConstraint : IHttpRouteConstraint
    {
            public static bool IsValidCustomerId(string sProduct)
            {
                return (!String.IsNullOrEmpty(sProduct) &&
                    sProduct.StartsWith("xyz-") &&
                    sProduct.Length > 5);
            }

            public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
            {
                object value;

                if (values.TryGetValue(parameterName, out value) && value != null)
                {
                    var stringVal = value as string;
                    if (!String.IsNullOrEmpty(stringVal))
                    {
                        // validate the account ID using the external function
                        return IsValidCustomerId(stringVal);
                    }
                }

                return false;
            }
    }
}