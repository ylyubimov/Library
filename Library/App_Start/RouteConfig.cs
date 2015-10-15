using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Library
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Login",
                url: "Admin/Login",
                defaults: new { controller = "Admin", action = "Login" }
            );

            routes.MapRoute(
                name: "Add",
                url: "Admin/Add",
                defaults: new { controller = "Admin", action = "Add" }
            );

            routes.MapRoute(
                name: "Edit",
                url: "Admin/Edit/{id}",
                defaults: new { controller = "Admin", action = "Edit", id = UrlParameter.Optional }
            );

        }
    }
}
