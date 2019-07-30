using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTestProject1.Models;

namespace WebApiTestProject1.Controllers
{
    [RoutePrefix("models")]
    public class ModelsController : ApiController
    {
        [HttpPost, Route("object")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [ValidateModelState(BodyRequired = true)] // METHOD 2: Use an action filter
        public IHttpActionResult Post([FromBody]MoreComplexTypeDto dto)
        {
            // METHOD 1: Inline model validation
            //if (!ModelState.IsValid)
            //{
            //    // Will put the failed model validations and put them in the reply
            //    return BadRequest(ModelState);
            //}

            return Ok("Posted data valid");
        }
    }
}
