using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestRP.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
              name: "nginxResource",
              routeTemplate: "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Nginx/Nginx/{resourceName}",
              defaults: new
              {
                  controller = "NginxResource",
                  action = RouteParameter.Optional,
                  resourceProvider = "Microsoft.Nginx",
                  resourceName = RouteParameter.Optional
              });

            routes.MapHttpRoute(
               name: "enumerateResources",
               routeTemplate: "subscriptions/{subscriptionId}/providers/Microsoft.Nginx/{resourceType}",
               defaults: new
               {
                   controller = "EnumerateResources",
                   action = RouteParameter.Optional,
                   resourceProvider = "Microsoft.Nginx"
               });

            routes.MapHttpRoute(
               name: "subscriptions",
               routeTemplate:
               "subscriptions/{subscriptionId}",
               defaults: new { controller = "Subscriptions", action = RouteParameter.Optional });

            routes.MapRoute(name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                });

        }
    }
}
