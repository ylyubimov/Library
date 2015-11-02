using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class PublishersController : Controller
    {
        // GET: Publishers
        public ActionResult Index()
        {
            using (LibraryContext db = new LibraryContext())
            {
                string a = Request["find"];
                if (!string.IsNullOrEmpty(a))
                {
                    return View(db.Publishers.Where(s => s.PublisherName != null && s.PublisherName.Contains(a)).ToList());
                }
                else
                {
                    return View(db.Publishers.ToList());
                }
            }
        }

        [Route("publishers/publisher/{id:int}")]
        public ActionResult Publisher(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Publisher b = db.Publishers.Find(id);
                if (b == null)
                {
                    return HttpNotFound();
                }
                return View(b);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("publishers/delete/{id:int}")]
        public ActionResult Delete(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Publisher publisher = db.Publishers.Find(id);
                if (publisher == null)
                {
                    return HttpNotFound();
                }
                return View(publisher);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, Publisher model)
        {
            using (LibraryContext db = new LibraryContext())
            {
                String linkedRecord = "";
                foreach (var record in db.Records)
                {
                    if (record.PublisherId == id)
                    {
                        linkedRecord = record.RecordName;
                    }
                }
                if (linkedRecord != "")
                {
                    ModelState.AddModelError("", "Этот издатель привязан к книге " + linkedRecord);
                    return View(db.Publishers.Find(id));
                }
                else
                {
                    db.Publishers.Remove((from p in db.Publishers where p.PublisherId == id select p).FirstOrDefault());
                    db.SaveChanges();
                    return Redirect("/Publishers/Index");
                }
            }
        }

        [HttpGet]
        [Authorize]
        [Route("publishers/edit/{id:int}")]
        public ActionResult Edit(int id)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Publisher publisher = db.Publishers.Find(id);
                if (publisher == null)
                {
                    return HttpNotFound();
                }
                return View(new PublisherEditModel(publisher));
            }
        }

        void editPublisher(Publisher publisher, PublisherEditModel model)
        {
            publisher.PublisherName = model.PublisherName;
            publisher.Address = model.Address;
            publisher.Number = model.Number;
            publisher.Email = model.Email;
        }

        [HttpPost]
        public ActionResult Edit(int id, PublisherEditModel model)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Publisher publisher = (from t in db.Publishers where t.PublisherId == id select t).FirstOrDefault();
                string currentName = publisher.PublisherName;
                if (model.PublisherName != currentName)
                {
                    var uniquePublisherQuery = (from t in db.Publishers
                                                where t.PublisherName == model.PublisherName
                                                select t).FirstOrDefault();
                    if (uniquePublisherQuery != null)
                    {
                        ModelState.AddModelError("PublisherName", "Издатель с таким названием уже существует");
                    }
                }
                if (ModelState.IsValid)
                {
                    editPublisher(publisher, model);
                    db.SaveChanges();
                    return Redirect("/Publishers/Index");
                }
                else
                {
                    return View(model);
                }
            }
        }

    }
}