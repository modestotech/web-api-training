using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTestProject1.Filters;

namespace WebApiTestProject1.Controllers
{
    [RoutePrefix("filters")]
    public class FiltersController : ApiController
    {
        [HttpGet, Route("")]
        [RouteTimerFilter("GettAllValues")] // Overrides the name
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet, Route("{id:int}")]
        public string Get(int id)
        {
            return "value-" + id;
        }
    }
}
