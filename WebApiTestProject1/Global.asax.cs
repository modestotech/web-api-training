using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebApiTestProject1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // To hide potentially harmful data that hackers could extract from the error
            // GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Default;

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            // If an error reaches this point, it should be logged, emailed, it's very critical if an error reaches this point
            // If the ExceptionHandler or ExceptionLogger throws an error, it'll end up here

            // Last resort error handler
            Context.Trace.Write("ERROR-- ", ex.ToString());
        }
    }
}
