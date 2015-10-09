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

        [HttpPost]
        public ActionResult Edit(int id, AdminViewModel viewModel)  // Используется AdminViewModel
        {
            using (LibraryContext db = new LibraryContext())
            {
                var recordQuery = (from r in db.Records
                             where r.RecordId == id
                             select r).First();
                var publisherQuery = (from p in db.Publishers
                                      where p.PublisherId == recordQuery.PublisherId
                                      select p).First();
                recordQuery.RecordName = viewModel.record.RecordName;
                recordQuery.RecordDescription = viewModel.record.RecordDescription;
                recordQuery.Author = viewModel.record.Author;
                publisherQuery.PublisherName = viewModel.publisher.PublisherName;
                publisherQuery.Address = viewModel.publisher.Address;
                publisherQuery.Email = viewModel.publisher.Email;
                publisherQuery.Number = viewModel.publisher.Number;
                recordQuery.Author = publisherQuery;
                db.SaveChanges();
                return Redirect("/Admin/Index");
            }
        }
    }
}