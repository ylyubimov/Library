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
                return View(db.Records.Where(s => s.RecordName!=null && s.RecordName.Contains(a) || s.RecordDescription!=null && s.RecordDescription.Contains(a)).ToList());
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

        private List<SelectListItem> GetPublishersList()
        {
            List<SelectListItem> publishers = new List<SelectListItem>();
            using (LibraryContext db = new LibraryContext())
            {
                var query = from p in db.Publishers orderby p.PublisherName select p;
                foreach (var q in query)
                {
                    publishers.Add(new SelectListItem { Text = q.PublisherName, Value = (q.PublisherId + 1).ToString() });
                    // для борьбы с 0-когда ничего не выбрали
                }
                return publishers;
            }
        }

        private void EditRecord(Record record, string name, string description, Publisher publisher)
        {
            record.RecordName = name;
            record.RecordDescription = description;
            record.Author = publisher;
        }

        private ActionResult WriteToDataBaseRecordAndPublisher(int id, AdminAddEditModel model, bool add)
        {
            bool edit = !add;
            using (LibraryContext db = new LibraryContext()) // Валидация книги
            {
                if ((db.Records.Where(rec => rec.RecordName == model.Record.RecordName).Count() > 0 && add)
                    || (!(db.Records.Where(rec => (rec.RecordName == model.Record.RecordName) && (rec.RecordId == id)).Count() > 0
                        || db.Records.Where(rec => (rec.RecordName == model.Record.RecordName)).Count() == 0)
                        && edit))
                {
                    ModelState.AddModelError("Record.RecordName", "Книга с таким названием уже существует");
                } // верная валидация = то же самое имя(его можно) || нет других имен из БД publishers
            }
            if (model.PublisherId != 0) // Воспользовались списком
            {
                if (ModelState.IsValidField("Record.RecordName") && ModelState.IsValidField("Record.RecordDiscription"))
                {
                    using (LibraryContext db = new LibraryContext())
                    {
                        int realPublisherId = model.PublisherId - 1;
                        if (edit)
                        {
                            var recordQuery = (from r in db.Records
                                               where r.RecordId == id
                                               select r).First();
                            Publisher publisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                            EditRecord(recordQuery, model.Record.RecordName, model.Record.RecordDescription, publisher);
                        }
                        else
                        {
                            model.Record.Author = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                            db.Records.Add(model.Record);
                        }
                        db.SaveChanges();
                        return Redirect("/Records/Index");
                    }
                }
                else
                {
                    model.Publishers = GetPublishersList();
                    return View(model);
                }
            }

            ModelState["PublisherId"].Errors.Clear(); // Игнорирования  [required] PublishersId в форме
            using (LibraryContext db = new LibraryContext())  // Валидация создаваемого издательства
            {
                if (db.Publishers.Where(rec => rec.PublisherName == model.Record.Author.PublisherName).Count() > 0)
                {
                    ModelState.AddModelError("Record.Author.PublisherName", "Издатель с таким названием уже существует");
                }
            }
            if (ModelState.IsValid) // С созданием нового publisher'a
            {
                using (LibraryContext db = new LibraryContext())
                {
                    if (edit)
                    {
                        var recordQuery = (from r in db.Records
                                           where r.RecordId == id
                                           select r).First();
                        EditRecord(recordQuery, model.Record.RecordName, model.Record.RecordDescription, model.Record.Author);
                    }
                    else
                    {
                        db.Records.Add(model.Record);
                    }
                    db.Publishers.Add(model.Record.Author);
                    db.SaveChanges();
                    return Redirect("/Records/Index");
                }
            }
            else
            {
                model.Publishers = GetPublishersList();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Add()
        {
            AdminAddEditModel model = new AdminAddEditModel();
            model.Publishers = GetPublishersList();
            return View(model);
        }


        [HttpPost]
        public ActionResult Add(AdminAddEditModel model, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
                file.SaveAs(System.IO.Path.Combine(path, model.Record.RecordName + ".pdf"));
            }            
            return WriteToDataBaseRecordAndPublisher(0, model, true);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            int realId;
            if (id == null)
            {
                return new HttpStatusCodeResult(400, "Expected books id");
            }
            else
            {
                realId = (int)id;
            }
            using (LibraryContext db = new LibraryContext())
            {
                bool idIsValid = (from r in db.Records
                                  select r.RecordId).Contains(realId);
                if (!idIsValid)
                {
                    return new HttpStatusCodeResult(404, "No book with such id: " + realId);
                }
                AdminAddEditModel model = new AdminAddEditModel();
                model.Publishers = GetPublishersList();
                model.Record = (from r in db.Records where r.RecordId == realId select r).First();
                model.Record.Author = new Publisher();
                model.PublisherId = model.Record.PublisherId + 1;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, AdminAddEditModel model)
        {
            return WriteToDataBaseRecordAndPublisher(id, model, false);
        }

        public FileResult downloadFile(int? id)
        {
            Record b = db.Records.Find(id);
            return File("../../Data/" + b.RecordName + ".pdf", "application/pdf");
        }

    }
}