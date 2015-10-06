using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Threading.Tasks;
using Library.Models;

namespace Library.Controllers
{
    public class AdminController : Controller
    {

        // GET: /Admin/
        public ActionResult Index()
        { 
            return View();
        }


    }
}