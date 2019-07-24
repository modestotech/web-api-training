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
        // [AcceptVerbs("GET", "VIEW")] // Any verb can be used, even non-standard verbs, this field seems to make the route not appear in Swagger doc
        [Route("~/prods")]
        /* 
         * Tilde overrides the route prefix attribute, so this row enables /prods, except /products, which is already available, 
           without the tilde the custom route would become /products/prods
        */
        public IEnumerable<string> Get()
        {
            return new string[] { "product1", "product2" };
        }

        // GET: api/products/5
        [HttpGet, Route("{id:int:range(1000,3000)}")]
        public string GetProduct(int id)
        {
            return "value";
        }

        // GET: api/products/5
        [HttpGet, Route("status/{status:alpha?}")]
        // Optional value is set with a question mark (is null if nothing is passed)
        // [HttpGet, Route("status/{status:alpha=}")]
        // Another way to achieve default value null
        // [HttpGet, Route("status/{status:alpha=pending}")]
        // Default value can be set here on the method signature, safer in the method signature as 
        // model binding might have some custom logic that in this case handles pending in some special way 
        public string GetProductsWithStatus(string status)
        {
            return String.IsNullOrEmpty(status) ? "NULL" : status;
        }

        // GET: api/products/5
        [HttpGet, Route("{id:int:range(1000,3000)}/orders/{custId:validCustomerId}")]
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
