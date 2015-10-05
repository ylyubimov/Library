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

        [HttpPost]
        public ActionResult Add(Record record)
        {  
            db.Records.Add(record);
            db.SaveChanges();    
            return View(db); // ?
        }

        [HttpPost]
        public ActionResult Edit(int id)
        {
            var query = from record in db.Records
                        where record.RecordId == id
                        select record;
            // todo
            return View();
        }
    }
}