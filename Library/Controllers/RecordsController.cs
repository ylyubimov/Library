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

        public ActionResult Index()
        {
            return View(db.Records.ToList());
        }

        public ActionResult Record(int? id)
        {
            Record b = db.Records.Find(id);
            if (b == null)
            {
                return HttpNotFound();
            }
           return View(b);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Record());
        }


        [HttpPost]
        public ActionResult Add(Record record)
        {
            using (LibraryContext db = new LibraryContext())
            {
                db.Records.Add(record);
                db.SaveChanges();

                return Redirect("/Record/Index");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Record b = db.Records.Find(id);
                return View(b);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Record record)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Record b = db.Records.Find(id);
                b.RecordName = record.RecordName;
                b.RecordDescription = record.RecordDescription;
                db.SaveChanges();

                return Redirect("/Record/Index");
            }

        }

    }
}