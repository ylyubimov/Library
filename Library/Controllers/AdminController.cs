using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Library.Models;
using Owin;

namespace Library.Controllers
{
    // [Authorize] todo: изменить модель базы - сделать с class MyUser:Admin{...}
    public class AdminController : Controller
    {
        public AdminController()
            : this(new UserManager<Admin>(new UserStore<Admin>(new AdminDbContext())))
        {
        }

        public AdminController(UserManager<Admin> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<Admin> UserManager { get; private set; }


        // GET: /Admin/
        public ActionResult Index()
        {   // Пока тут вывод информации об админе
            using (var db = new AdminDbContext())
            {
                var user = new Admin() { UserName = "abacaba" };

                // Store Gender as Claim
                user.Claims.Add(new IdentityUserClaim());

                //var result = await UserManager.CreateAsync(user, "dabacaba");
                return View(user);
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

        private async Task SignInAsync(Admin user, bool isPersistent)
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

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Record());
        }


        private void AddErrorsProcessing(Record record)
        {
            using (LibraryContext db = new LibraryContext())
            {
                if (db.Records.Where(rec => rec.RecordName == record.RecordName).Count() > 0)
                {
                    ModelState.AddModelError("RecordName", "Книга с таким названием уже существует");
                }

                // todo: проверка ниже нужна для будущего выпадающего списка: чтобы не создать нового publishera, если он существует
                if (db.Publishers.Where(rec => rec.PublisherName == record.Author.PublisherName).Count() > 0)
                {
                    ModelState.AddModelError("Author.PublisherName", "Издатель с таким названием уже существует");
                }
            }
        }

        [HttpPost]
        public ActionResult Add(Record record)
        { 
            AddErrorsProcessing(record);

            if (ModelState.IsValid)
            {
                using (LibraryContext db = new LibraryContext())
                {           
                    db.Publishers.Add(record.Author);
                    db.Records.Add(record);
                    db.SaveChanges();
                    return Redirect("/Admin/Index");
                }
            }
            else
            {
                return View(record);
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
                Record currentBook = (from r in db.Records where r.RecordId == realId select r).First();
                Publisher currentPublisher = (from p in db.Publishers where p.PublisherId == currentBook.PublisherId select p).First();
                AdminEditModel viewModel = new AdminEditModel(currentBook, currentPublisher);
                return View(viewModel);
            }
        }

        private void ChangeEntities(Record record, Publisher publisher, AdminEditModel model)
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
        public ActionResult Edit(int id, AdminEditModel viewModel)
        {
            if (ModelState.IsValid)
            {
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
            }
            else
            {
                return View(viewModel);
            }
        }
    }
}