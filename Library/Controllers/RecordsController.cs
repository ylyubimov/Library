using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using System.Net;

namespace Library.Controllers
{
    public class RecordsController : Controller
    {
        private LibraryContext db = new LibraryContext();

        //
        // GET: /Books/
        public ActionResult Index()
        {
            for (int i = 0; i < 10; i++)
            {
                Record a = new Record();
                a.RecordName = i.ToString();
                a.RecordId = i;
                db.Records.Add(a);
            }
            return View(db.Records.Local.ToList());
        }

        public ActionResult Record(int? id)
        {
            for (int i = 0; i < 10; i++)
            {
                Record a = new Record();
                a.RecordName = i.ToString();
                a.RecordId = i;
                db.Records.Add(a);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Record b = db.Records.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }
           return View(b);
        }
    }
}