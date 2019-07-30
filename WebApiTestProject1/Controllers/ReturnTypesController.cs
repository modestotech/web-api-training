using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiTestProject1.Filters;
using WebApiTestProject1.Models;

namespace WebApiTestProject1.Controllers
{
    [RoutePrefix("returntypes")]
    [ClientCacheControlFilter(ClientCacheControl.NoCache)] // For enabling breakpointing into chain
    public class ReturnTypesController : ApiController
    {
        [HttpGet, Route("void")]
        public void ReturnVoid()
        {
        }

        [HttpGet, Route("object")]
        public ComplexTypeDto GetObject()
        {
            var dto = new ComplexTypeDto()
            {
                String1 = "This is string 1",
                String2 = "This is string 2",
                Int1 = 55,
                Int2 = 66,
                Date1 = new DateTime()
            };

            // Error response
            throw new InvalidOperationException("I'm sorry, Dave, I'm afraid I can't do that");

            return dto;
        }

        [HttpGet, Route("httpresponse")]
        [ResponseType(typeof(ComplexTypeDto))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ComplexTypeDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(HttpError))]
        public HttpResponseMessage GetAnHttpResponse()
        {
            var dto = new ComplexTypeDto()
            {
                String1 = "This is string 1",
                String2 = "This is string 2",
                Int1 = 55,
                Int2 = 66,
                Date1 = new DateTime()
            };

            //var response = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    // Note that now I'm responsible for content negotiation
            //    // This alternative will confuse a caller wanting XML or other data type
            //    Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            //};

            // Or as a better alternative
            // Knows about request header and formatters so that it does content negotiation
            var response = Request.CreateResponse(dto);

            response.Headers.Add("X-MyCustomHeader", "MyHeaderValue");
            response.ReasonPhrase = "Most Excellent!";

            // Enables returning a complex type but also mess with the Response key values

            // Error response
            response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something invalid");

            // response = Request.CreateErrorResponse(HttpStatusCode)

            return response;
        }
    }
}
