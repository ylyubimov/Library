using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Threading.Tasks;
using Library.Models;
using System.Data.Entity.Validation;

namespace Library.Controllers
{
    // TODO: [Authorize(Roles ="Admin")] - разобраться с Roles, Authorize
    public class AdminController : Controller
    {

        // GET: /Admin/
        public ActionResult Index()
        {   // Пока тут вывод информации об админе
            using (var db = new LibraryContext()) {
                var adminInfo = (from admin in db.LibraryAdmins
                                 select admin).First();
                return View(adminInfo);
            }            
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Record());
        }


        [HttpPost]
        public ActionResult Add(Record record)
        {
            try {
                using (LibraryContext db = new LibraryContext())
                {
                    record.RecordId = db.Records.Count();
                    record.PublisherId = db.Publishers.Count();
                    record.Author.PublisherId = record.PublisherId;
                    db.Publishers.Add(record.Author);
                    db.Records.Add(record);
                    db.SaveChanges();
                    return Redirect("/Admin/Index");
                }
            } catch (DbEntityValidationException e) // Отлавливается некорректно введенная форма
            {
                var error = e.EntityValidationErrors.First().ValidationErrors.First();
                this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(record);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id) // Используется AdminViewModel
        {
            using (LibraryContext db = new LibraryContext())
            {
                bool idIsValid = (from r in db.Records
                                  select r.RecordId).Contains(id);
                if (!idIsValid)
                {
                    return new HttpNotFoundResult();
                }
                Record currentBook = (from r in db.Records where r.RecordId == id select r).First();
                Publisher currentPublisher = (from p in db.Publishers where p.PublisherId == currentBook.PublisherId select p).First();
                AdminViewModel viewModel = new AdminViewModel(currentBook, currentPublisher);
                return View(viewModel);
            }
        } 

        private void ChangeEntities(Record record, Publisher publisher, AdminViewModel model)
        {
            record.RecordName = model.record.RecordName;
            record.RecordDescription = model.record.RecordDescription;
            record.Author = model.record.Author;
            publisher.PublisherName = model.publisher.PublisherName;
            publisher.Address = model.publisher.Address;
            publisher.Email = model.publisher.Email;
            publisher.Number = model.publisher.Number;
            record.Author = publisher;
        }

        [HttpPost]
        public ActionResult Edit(int id, AdminViewModel viewModel)  // Используется AdminViewModel
        {
            try {
                using (LibraryContext db = new LibraryContext())
                {
                    var recordQuery = (from r in db.Records
                                       where r.RecordId == id
                                       select r).First();
                    var publisherQuery = (from p in db.Publishers
                                          where p.PublisherId == recordQuery.PublisherId
                                          select p).First();
                    ChangeEntities(recordQuery, publisherQuery, viewModel);           
                    db.SaveChanges();
                    return Redirect("/Admin/Index");
                }
            } catch (DbEntityValidationException e)
            {
                var error = e.EntityValidationErrors.First().ValidationErrors.First();
                this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                return View(viewModel);
            }
        }
    }
}