using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace InventoryManagementBackend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Force JSON over XML
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));

            // Remove XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Route config
            config.MapHttpAttributeRoutes();
           
        }
    }
}
