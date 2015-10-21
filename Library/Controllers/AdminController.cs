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
    }
}