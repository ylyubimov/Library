using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class PublishersController : Controller
    {
        // GET: Publishers
        public ActionResult Index()
        {
            using (LibraryContext db = new LibraryContext())
            {
                string a = Request["find"];
                if (!String.IsNullOrEmpty(a))
                {
                    return View(db.Publishers.Where(s => s.PublisherName != null && s.PublisherName.Contains(a)).ToList());
                }
                else
                {
                    return View(db.Publishers.ToList());
                }
            }
        }

        public ActionResult Publisher(int? id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Publisher b = db.Publishers.Find(id);
                if (b == null)
                {
                    return HttpNotFound();
                }
                return View(b);
            }
        }

    }
}