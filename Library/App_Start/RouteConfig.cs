﻿using System;
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
                name: "Record",
                url: "Records/Record/{id}",
                defaults: new { controller = "Records", action = "Record" }
            );

            routes.MapRoute(
               name: "AddRecord",
               url: "Admin/Add",
               defaults: new { controller = "Admin", action = "Add" }
            );

            routes.MapRoute(
               name: "EditRecord",
               url: "Admin/Edit/{id}", 
               defaults: new { controller = "Admin", action = "Edit"}
            );

            routes.MapRoute(
               name: "IndexAdmin",
               url: "Admin",
               defaults: new { controller = "Admin", action = "Index" }
            );
        }
    }
}
