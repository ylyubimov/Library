using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Library.Models;
using System.Data.Entity;

namespace Library
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LibraryContext db = new LibraryContext();
            db.Database.Initialize(true);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
