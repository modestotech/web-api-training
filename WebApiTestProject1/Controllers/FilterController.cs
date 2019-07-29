using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTestProject1.Filters;

namespace WebApiTestProject1.Controllers
{
    [ActionFilterTemplate] // Declare a filter to use for this controller
    public class FilterController : ApiController
    {
        [ActionFilterTemplate]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [OverrideActionFilters] // Override any action filter declared globally
        [ActionFilterTemplate] // And declare a filter to use here
        public string Get(int id)
        {
            return "value-" + id;
        }
    }
}
