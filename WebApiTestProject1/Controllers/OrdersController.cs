using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiTestProject1.Controllers
{
    [RoutePrefix("orders")]
    public class OrdersController : ApiController
    {
        /*
         * Web Api has several sets of rules to determine route order. 
         * In this case orders/complete falls into GetOrderById as it matches the length, 
         * when it should go to GetOrderWithStatus.
         * The best solution is to set up routing in a way that makes conflicts not possible.
         * If that's not possible, use the Order property,
         * It defaults to 0 and the lower number comes first.
        */
        // GET: orders/5
        [HttpGet, Route("{id:length(8)}", Order = 2)]
        public string GetOrderById(string id)
        {
            return "order-" + id;
        }

        // GET: orders
        [HttpGet, Route("{status:regex(^(?i)(new|complete|pending)$)}", Order = 1)]
        public IEnumerable<string> GetOrderWithStatus()
        {
            return new string[] { "status1", "status2" };
        }

        // GET: orders/date/7/25/2019
        // The asterisk tells WebApi to bind to the parameter using all of the remaining segments.
        [HttpGet, Route("date/{*dateInput:datetime}")]
        public IEnumerable<string> GetOrderWithDate(DateTime dateInput)
        {
            return new string[] { "order1-" + dateInput.ToShortDateString(), "order2-" + dateInput.ToShortDateString()};
        }
    }
}
