using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTestProject1.Filters;

namespace WebApiTestProject1.Controllers
{

    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        // GET: api/Auth
        [HttpGet, Route("")]
        // [Authorize] // Flag to check if a proper IPrinciple is set, if not set in WebApiConfig.cs
        // [AllowAnonymous] // Overrides the authorize from the WebApiConfig layer to allow anonymous
        // [OverrideAuthentication] // Overrides the complete authentication stack for this route
        // [JwtAuthenticationFilter] // Applies the JwtAuthenticationFilter for this route
        // [RequireHttps] // For requiring https as authorization
        // [RequireApiKey("W")]
        public IEnumerable<string> Get()
        {
            throw new InvalidOperationException();
            return new string[] { User.Identity.Name, User.Identity.AuthenticationType };
        }

        // GET: api/Auth/5
        [HttpGet, Route("{id:int}")]
        public string Get(int id)
        {
            throw new ArgumentOutOfRangeException("id", "IDs must be in the range 1 to 50");
            return "value";
        }

        // POST: api/Auth
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT: api/Auth/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Auth/5
        public void Delete(int id)
        {
        }
    }
}
