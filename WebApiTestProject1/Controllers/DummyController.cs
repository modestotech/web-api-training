using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WebApiTestProject1.Models;

namespace WebApiTestProject1.Controllers
{
    [RoutePrefix("dummy")]
    public class DummyController : ApiController
    {
        // GET: dummy/date/7/25/2019
        [HttpGet, Route("date/{*dateInput:datetime}")]
        // The asterisk tells WebApi to bind to the parameter using all of the remaining segments.
        public IEnumerable<string> GetOrderWithDate(DateTime dateInput)
        {
            return new string[] { "dummy1-" + dateInput.ToShortDateString(), "dummy2-" + dateInput.ToShortDateString() };
        }

        // GET: segments/string/to/concatenate
        [HttpGet, Route("segments/{*array:maxlength(256)}")]
        // WebAPI doesn't know how to bind a string array, so it's necessary with a custom model binder.
        public string[] GetSegments([ModelBinder(typeof(StringArrayWildcardBinder))] string[] array)
        {
            return array;
        }
    }
}
