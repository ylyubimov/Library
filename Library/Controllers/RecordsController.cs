using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using System.Net;

namespace Library.Controllers
{
    public class RecordsController : Controller // todo: добавить в форму string AuthorName
    {
        private LibraryContext db = new LibraryContext();

        public ActionResult Modal(int? id)
        {
            Record b = db.Records.Find(id);
            return PartialView("_Modal", new { firstName = "bdfy", lastName = "bdfysx" });
        }

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
            record.RecordPublisher = publisher;
        }

        private ActionResult WriteToDataBaseRecordAndPublisher(int id, AdminAddEditModel model, bool add)
        {
            bool edit = !add;
            if (model.PublisherId != 0) // Воспользовались списком
            {
                if (ModelState.IsValidField("Record.RecordName") && ModelState.IsValidField("Record.RecordDescription"))
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
                            model.Record.RecordPublisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
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
                if (db.Publishers.Where(rec => rec.PublisherName == model.Record.RecordPublisher.PublisherName).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("Record.RecordPublisher.PublisherName", "Издатель с таким названием уже существует");
                }
            }
            if (ModelState.IsValid) // todo: убрать Errors().clear() + добавить просто валидацию полей, а не всей модели!! С созданием нового publisher'a
            {
                using (LibraryContext db = new LibraryContext())
                {
                    if (edit)
                    {
                        var recordQuery = (from r in db.Records
                                           where r.RecordId == id
                                           select r).First();
                        EditRecord(recordQuery, model.Record.RecordName, model.Record.RecordDescription, model.Record.RecordPublisher);
                    }
                    else
                    {
                        db.Records.Add(model.Record);
                    }
                    db.Publishers.Add(model.Record.RecordPublisher);
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
                model.Record.RecordPublisher = new Publisher();
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