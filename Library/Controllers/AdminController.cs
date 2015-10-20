using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Library.Models;

namespace Library.Controllers
{
    // [Authorize] todo: изменить модель базы - сделать с class MyUser:IdentityUser{...}
    public class AdminController : Controller
    {
        ////public AdminController()
        ////{
        ////    IdentityDbContext<IdentityUser> db = new IdentityDbContext<IdentityUser>();
        ////    adminManager = new adminManager<IdentityUser>(new UserStore<IdentityUser>(db));
        ////}

        ////public adminManager<IdentityUser> adminManager { get; private set; } 


        //public AdminController()
        //    : this(new adminManager<IdentityUser>(new UserStore<IdentityUser>(new MyDbContext())))
        //{
        //}

        //public AdminController(adminManager<IdentityUser> adminManager)
        //{
        //    adminManager = adminManager;
        //}

        //public adminManager<IdentityUser> adminManager { get; private set; }
        //// GET: /Admin/
        public ActionResult Index(AdminLoginModel model)
        {   // Пока тут вывод информации об админе
            //using (var db = new ApplicationDbContext())
            //{
            //    var adminInfo = (from admin in db.Users
            //                     select admin).First();
            //    return View(adminInfo);
            //}
            //var db = new AdminLoginModel();
            return View(model);
        }

        //// ===============================================================================
        //// GET: /Admin/Login
        //[AllowAnonymous]
        //public ActionResult Login() 
        //{
        //    return View(new AdminLoginModel());
        //}

        ////
        //// POST: /Admin/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(AdminLoginModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await adminManager.FindAsync(model.Login, model.Password);
        //        if (user != null)
        //        {
        //            await SignInAsync(user, model.RememberMe);
        //            return RedirectToAction("Admin", "Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid username or password.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        private AdminSignInManager _signInManager;
        private AdminManager _userManager;

        public AdminController()
        {
        }

        public AdminController(AdminManager userManager, AdminSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public AdminSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AdminSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public AdminManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AdminManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AdminLoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        //private async Task SignInAsync(IdentityUser user, bool isPersistent)
        //{
        //    HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Admin");
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