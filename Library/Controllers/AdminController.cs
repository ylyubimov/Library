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
        private LibraryContext db = new LibraryContext();

        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Record());
        }


        [HttpPost]
        public ActionResult Add(Record record)
        {
            record.RecordId = db.Records.Count();
            db.Records.Add(record);
            db.SaveChanges();
            return Redirect("/Admin/Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            bool idIsValid = (from r in db.Records
                            select r.RecordId).Contains(id);
            if (!idIsValid)
            {
                return new HttpNotFoundResult();
            }
            return View(new Record());
        }

        [HttpPost]
        public ActionResult Edit(int id, Record record)
        {
            var query = (from r in db.Records
                            where r.RecordId == id
                            select r).First();
            query.RecordName = record.RecordName;
            query.RecordDescription = record.RecordDescription;
            db.SaveChanges();
            return Redirect("/Admin/Index");
        }
    }
}