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
            String a = Request["find"];
            if (!String.IsNullOrEmpty(Request["find"]))
            {
                return View(db.Records.Where(s => s.RecordName!=null && s.RecordName.Contains(a)).ToList());
            }
            else
            {
                return View(db.Records.ToList());
            }
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
        public ActionResult Add(Record record, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            using (LibraryContext db = new LibraryContext())
            {
                record.RecordId = db.Records.Count();
                record.PublisherId = db.Publishers.Count();
                record.Author.PublisherId = record.PublisherId;
                db.Publishers.Add(record.Author);
                db.Records.Add(record);
                db.SaveChanges();
            }
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
                file.SaveAs(System.IO.Path.Combine(path, record.RecordName+".pdf"));
            }
            return Redirect("/Records/Index");
        }

        public FileResult downloadFile(int? id)
        {
            Record b = db.Records.Find(id);
            return File("../../Data/"+b.RecordName+".pdf", "application/pdf");
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