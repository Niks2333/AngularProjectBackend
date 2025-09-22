using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InventoryManagementBackend
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)

        {


            var origin = HttpContext.Current.Request.Headers["Origin"];

            // Allow only Angular dev server
            if (origin == "http://localhost:4200")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);
            }
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")

            {

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods",

                "GET, POST, PUT, DELETE");

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers",

                "Content-Type, Accept, Authorization");

                HttpContext.Current.Response.End();

            }


        }

    }
}
