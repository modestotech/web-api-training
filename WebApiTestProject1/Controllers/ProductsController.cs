using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiTestProject1.Controllers
{
    // Using attribute routes to have all in one place in a more understandable and predictable way
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {

        // GET: api/products
        [HttpGet, Route("")]
        public IEnumerable<string> Get()
        {
            return new string[] { "product1", "product2" };
        }

        // GET: api/products/5
        [HttpGet, Route("{id:int:range(1000,3000)}")]
        public string Get(int id)
        {
            return "value";
        }

        // GET: api/products/5
        [HttpGet, Route("{id:int:range(1000,3000)}/orders/{custId}")]
        public string Get(int id, string custId)
        {
            return "product-orders-" + custId;
        }

        // POST: api/products
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/products/5
        [HttpPut, Route("{id:int:range(1000,3000)}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/products/5
        [HttpDelete, Route("{id:int:range(1000,3000)}")]
        public void Delete(int id)
        {
        }
    }
}
