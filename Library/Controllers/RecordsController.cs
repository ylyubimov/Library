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
                return View(db.Records.Where(s => s.RecordName != null && s.RecordName.Contains(a) || s.RecordDescription != null && s.RecordDescription.Contains(a)).ToList());
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

        private void EditRecord(Record record, string name, string description, string author, Publisher publisher)
        {
            record.RecordName = name;
            record.RecordDescription = description;
            record.AuthorName = author;
            record.RecordPublisher = publisher;
        }

        private void EditPublisher(Publisher publisher, string name, string address, string number, string email)
        {
            publisher.PublisherName = name;
            publisher.Address = address;
            publisher.Number = number;
            publisher.Email = email;
        }

        private void LoadPdf(string name, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            if (fileUpload == null)
            {
                return;

            }
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
                file.SaveAs(System.IO.Path.Combine(path, name + ".pdf"));
            }
        }

        public FileResult downloadFile(int? id)
        {
            Record b = db.Records.Find(id);
            return File("../../Data/" + b.RecordName + ".pdf", "application/pdf");
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
            using (LibraryContext db = new LibraryContext())
            {
                if (!ModelState.IsValidField("RecordName") || !ModelState.IsValidField("RecordAuthor"))
                {
                    model.Publishers = GetPublishersList();
                    return View(model);
                }
                if (model.PublisherId != 0) // Воспользовались списком
                {
                    int realPublisherId = model.PublisherId - 1;
                    Record newRecord = new Record();
                    Publisher publisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                    EditRecord(newRecord, model.RecordName, model.RecordDescription, model.RecordAuthor, publisher);
                    db.Records.Add(newRecord);
                    db.SaveChanges();
                    LoadPdf(newRecord.RecordName, fileUpload);
                    return Redirect("/Records/Index");
                }
                // Новое издательство. Валидация создаваемого издательства
                if (db.Publishers.Where(rec => rec.PublisherName == model.PublisherName).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("PublisherName", "Издатель с таким названием уже существует");
                }
                if (ModelState.IsValidField("PublisherName") && ModelState.IsValidField("PublisherEmail"))
                {
                    Publisher newPublisher = new Publisher();
                    EditPublisher(newPublisher, model.PublisherName, model.PublisherAddress, model.PublisherNumber, model.PublisherEmail);
                    Record newRecord = new Record();
                    EditRecord(newRecord, model.RecordName, model.RecordDescription, model.RecordAuthor, newPublisher);
                    db.Records.Add(newRecord);
                    db.Publishers.Add(newPublisher);
                    db.SaveChanges();
                    LoadPdf(newRecord.RecordName, fileUpload);
                    return Redirect("/Records/Index");
                }
                else
                {
                    model.Publishers = GetPublishersList();
                    return View(model);
                }
            }
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
                Record baseRecord = (from r in db.Records where r.RecordId == realId select r).First();
                model.RecordName = baseRecord.RecordName;
                model.RecordDescription = baseRecord.RecordDescription;
                model.RecordAuthor = baseRecord.AuthorName;
                model.PublisherId = baseRecord.PublisherId + 1;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, AdminAddEditModel model)
        {
            using (LibraryContext db = new LibraryContext())
            {
                if (!ModelState.IsValidField("RecordName") || !ModelState.IsValidField("RecordAuthor"))
                {
                    model.Publishers = GetPublishersList();
                    return View(model);
                }
                if (model.PublisherId != 0) // Воспользовались списком
                {
                    int realPublisherId = model.PublisherId - 1;
                    var recordQuery = (from r in db.Records
                                       where r.RecordId == id
                                       select r).First();
                    Publisher publisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                    EditRecord(recordQuery, model.RecordName, model.RecordDescription, model.RecordAuthor, publisher);
                    db.SaveChanges();
                    /* todo: Load pdf here*/
                    return Redirect("/Records/Index");
                }
                // Новое издательство. Валидация создаваемого издательства
                if (db.Publishers.Where(rec => rec.PublisherName == model.PublisherName).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("PublisherName", "Издатель с таким названием уже существует");
                }
                if (ModelState.IsValidField("PublisherName") && ModelState.IsValidField("PublisherEmail"))
                {
                    Publisher newPublisher = new Publisher();
                    EditPublisher(newPublisher, model.PublisherName, model.PublisherAddress, model.PublisherNumber, model.PublisherEmail);
                    var recordQuery = (from r in db.Records
                                       where r.RecordId == id
                                       select r).First();
                    EditRecord(recordQuery, model.RecordName, model.RecordDescription, model.RecordAuthor, newPublisher);
                    db.Publishers.Add(newPublisher);
                    db.SaveChanges();
                    /* todo: Load pdf here*/
                    return Redirect("/Records/Index");
                }
                else
                {
                    model.Publishers = GetPublishersList();
                    return View(model);
                }
            }
        }
    }
}