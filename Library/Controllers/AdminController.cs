using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Threading.Tasks;
using Library.Models;
using System.Data.Entity.Validation;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Security;
using Microsoft.Owin.Security;

namespace Library.Controllers
{
    // [Authorize] todo: изменить модель базы - сделать с class MyUser:IdentityUser{...}
    public class AdminController : Controller
    {
        public AdminController()
        {
            IdentityDbContext<IdentityUser> db = new IdentityDbContext<IdentityUser>();
            UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(db));
        }

        public UserManager<IdentityUser> UserManager { get; private set; }

        // GET: /Admin/
        public ActionResult Index()
        {   // Пока тут вывод информации об админе
            using (var db = new LibraryContext())
            {
                var adminInfo = (from admin in db.LibraryAdmins
                                 select admin).First();
                return View(adminInfo);
            }
        }

        // ===============================================================================
        // GET: /Admin/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new AdminLoginModel());
        }

        //
        // POST: /Admin/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Login, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToAction("Admin", "Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        //
        // POST: /Admin/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Records", "Index");
        }

        // ==============================================================

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
                            // todo: make function of this block
                            recordQuery.RecordName = model.Record.RecordName;
                            recordQuery.RecordDescription = model.Record.RecordDescription;
                            recordQuery.Author = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                        }
                        else
                        {
                            model.Record.Author = (from p in db.Publishers where p.PublisherId == realPublisherId select p).First();
                            db.Records.Add(model.Record);
                        }
                        db.SaveChanges();
                        return Redirect("/Admin/Index");
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
                        recordQuery.RecordName = model.Record.RecordName;
                        recordQuery.RecordDescription = model.Record.RecordDescription;
                        recordQuery.Author = model.Record.Author;
                    }
                    else
                    {
                        db.Records.Add(model.Record);
                    }
                    db.Publishers.Add(model.Record.Author);
                    db.SaveChanges();
                    return Redirect("/Admin/Index");
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
        public ActionResult Add(AdminAddEditModel model)
        {
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
    }
}