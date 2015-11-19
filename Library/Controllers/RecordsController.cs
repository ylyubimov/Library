using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using System.Net;
using System.IO;

namespace Library.Controllers
{
    public class RecordsController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Modal(int? id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Record b = db.Records.Find(id);
                return PartialView("_Modal", new { firstName = "bdfy", lastName = "bdfysx" });
            }
        }

        [HttpGet]
        public ActionResult Test()
        {
            return PartialView("_Modal");
        }

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
                return PartialView("_Modal");
            using (LibraryContext db = new LibraryContext())
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
        }

        [Route("records/record/{id:int}")]
        public ActionResult Record(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Record b = db.Records.Find(id);
                if (b == null)
                {
                    return HttpNotFound();
                }
                return View(b);
            }
        }

        private List<SelectListItem> getPublishersList()
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

        [Authorize]
        private void editRecord(Record record, AdminAddEditModel model)
        {
            record.RecordName = model.RecordName;
            record.RecordDescription = model.RecordDescription;
            record.AuthorName = model.RecordAuthor;
            record.ISBN = model.ISBN;
            record.NumberOfPages = model.NumberOfPages;
            record.Annotation = model.Annotation;
            record.CreationDate = model.CreationDate;
            record.Recomended = model.Recomended;
        }

        [Authorize]
        private void editPublisher(Publisher publisher, string name, string address, string number, string email)
        {
            publisher.PublisherName = name;
            publisher.Address = address;
            publisher.Number = number;
            publisher.Email = email;
        }

        [Authorize]
        private void loadPdf(string name, IEnumerable<HttpPostedFileBase> fileUpload)
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
                System.IO.Directory.SetCurrentDirectory(path);
                GhostscriptSharp.GhostscriptWrapper.GeneratePageThumb(name + ".pdf", name + ".png", 1, 100, 100);
            }
        }

        public FileResult downloadFile(int? id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Record b = db.Records.Find(id);
                return File("../../Data/" + b.ISBN + ".pdf", "application/pdf");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            AdminAddEditModel model = new AdminAddEditModel();
            model.Publishers = getPublishersList();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AdminAddEditModel model, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            using (LibraryContext db = new LibraryContext())
            {
                if (!ModelState.IsValidField("RecordName") || !ModelState.IsValidField("RecordAuthor") || !ModelState.IsValidField("ISBN"))
                {
                    model.Publishers = getPublishersList();
                    return View(model);
                }
                if (model.PublisherId != 0) // Воспользовались списком
                {
                    int realPublisherId = model.PublisherId - 1;
                    Record newRecord = new Record();
                    newRecord.RecordPublisher = new Publisher();
                    editRecord(newRecord, model);
                    newRecord.RecordPublisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                    db.Records.Add(newRecord);
                    db.SaveChanges();
                    loadPdf(newRecord.ISBN, fileUpload);
                    return Redirect("/Records/Index");
                }
                // Новое издательство. Валидация создаваемого издательства
                if (db.Publishers.Where(rec => rec.PublisherName == model.PublisherName).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("PublisherName", "Издатель с таким названием уже существует");
                }
                if (ModelState.IsValidField("PublisherName") && ModelState.IsValidField("PublisherEmail"))
                {
                    Record newRecord = new Record();
                    newRecord.RecordPublisher = new Publisher();
                    editRecord(newRecord, model);
                    editPublisher(newRecord.RecordPublisher, model.PublisherName, model.PublisherAddress, model.PublisherNumber, model.PublisherEmail);
                    db.Records.Add(newRecord);
                    db.Publishers.Add(newRecord.RecordPublisher);
                    db.SaveChanges();
                    loadPdf(newRecord.ISBN, fileUpload);
                    return Redirect("/Records/Index");
                }
                else
                {
                    model.Publishers = getPublishersList();
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Authorize]
        [Route("records/edit/{id:int}")]
        public ActionResult Edit(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                bool idIsValid = (from r in db.Records
                                  select r.RecordId).Contains(id);
                if (!idIsValid)
                {
                    return new HttpStatusCodeResult(404, "No book with such id: " + id);
                }
                AdminAddEditModel model = new AdminAddEditModel();
                model.Publishers = getPublishersList();
                Record baseRecord = (from r in db.Records where r.RecordId == id select r).First();
                model.RecordName = baseRecord.RecordName;
                model.RecordDescription = baseRecord.RecordDescription;
                model.RecordAuthor = baseRecord.AuthorName;
                model.PublisherId = baseRecord.PublisherId + 1;
                model.Annotation = baseRecord.Annotation;
                model.CreationDate = baseRecord.CreationDate;
                model.ISBN = baseRecord.ISBN;
                model.NumberOfPages = baseRecord.NumberOfPages;
                model.Recomended = baseRecord.Recomended;
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int id, AdminAddEditModel model, IEnumerable<HttpPostedFileBase> fileUpload)
        {
            using (LibraryContext db = new LibraryContext())
            {
                if (!ModelState.IsValidField("RecordName") || !ModelState.IsValidField("RecordAuthor") || !ModelState.IsValidField("ISBN"))
                {
                    model.Publishers = getPublishersList();
                    return View(model);
                }
                if (model.PublisherId != 0) // Воспользовались списком
                {
                    int realPublisherId = model.PublisherId - 1;
                    var recordQuery = (from r in db.Records
                                       where r.RecordId == id
                                       select r).First();
                    editRecord(recordQuery, model);
                    recordQuery.RecordPublisher = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                    db.SaveChanges();
                    loadPdf(recordQuery.ISBN, fileUpload);
                    return Redirect("/Records/Index");
                }
                // Новое издательство. Валидация создаваемого издательства
                if (db.Publishers.Where(rec => rec.PublisherName == model.PublisherName).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("PublisherName", "Издатель с таким названием уже существует");
                }
                if (ModelState.IsValidField("PublisherName") && ModelState.IsValidField("PublisherEmail"))
                {
                    var recordQuery = (from r in db.Records
                                       where r.RecordId == id
                                       select r).First();
                    editRecord(recordQuery, model);
                    editPublisher(recordQuery.RecordPublisher, model.PublisherName, model.PublisherAddress, model.PublisherNumber, model.PublisherEmail);
                    db.Publishers.Add(recordQuery.RecordPublisher);
                    db.SaveChanges();
                    loadPdf(recordQuery.ISBN, fileUpload);
                    return Redirect("/Records/Index");
                }
                else
                {
                    model.Publishers = getPublishersList();
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Authorize]
        [Route("records/delete/{id:int}")]
        public ActionResult Delete(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                var recordToDelete = (from r in db.Records where r.RecordId == id select r).FirstOrDefault();
                if (recordToDelete == null)
                {
                    return new HttpStatusCodeResult(404, "No book with such id: " + id);
                }
                else
                {
                    return View(new AdminDeletionModel(recordToDelete, recordToDelete.RecordPublisher));
                }
            }
        }


        private void removeFiles(Record record)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Data/";
            if (System.IO.File.Exists(System.IO.Path.Combine(path, record.ISBN + ".pdf")))
            {
                System.IO.File.Delete(System.IO.Path.Combine(path, record.ISBN + ".pdf"));
                System.IO.File.Delete(System.IO.Path.Combine(path, record.ISBN + ".png"));
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, AdminDeletionModel model)
        {
            using (LibraryContext db = new LibraryContext())
            {
                var record = (from r in db.Records where r.RecordId == id select r).FirstOrDefault();
                db.Records.Remove(record);
                removeFiles(record);
                db.SaveChanges();
                return Redirect("/Records/Index");
            }
        }
    }
}