using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WebApiTestProject1.Handlers;
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

        // GET: dummy/segments/string/to/concatenate
        [HttpGet, Route("segments/{*array:maxlength(256)}")]
        // WebAPI doesn't know how to bind a string array, so it's necessary with a custom model binder.
        public string[] GetSegments([ModelBinder(typeof(StringArrayWildcardBinder))] string[] array)
        {
            return array;
        }

        [HttpGet, Route("clienturl/{id:int}", Name = "GetById")]
        public string Get(int id)
        {
            return "id-" + id;
        }

        // GET: dummy/clienturl
        [HttpGet, Route("clienturl")]
        // WebAPI doesn't know how to bind a string array, so it's necessary with a custom model binder.
        public IEnumerable<string> Get()
        {
            var getByIdUrl = Url.Link("GetById", new { id = 123 });
            var serverViewBaseUrl = Request.GetSelfReferenceBaseUrl();
            var clientViewUrl = Request.RebaseUrlForClient(new Uri(getByIdUrl));
            var clientIP = Request.GetClientIpAddress();

            return new string[]
            {
                getByIdUrl, // Url from server's perspective
                serverViewBaseUrl.ToString(), // Base url from server's perspective
                clientViewUrl.ToString(), // Url from client's perspective, different if server is behind proxy (load balancer etc)
                clientIP, // Client IP address
            };
        }
    }
}
