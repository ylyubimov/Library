using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Library.Models;

namespace Library
{
    public class AdminManager : UserManager<Admin>
    {
        public AdminManager(IUserStore<Admin> store)
            : base(store)
        {
        }

        public static AdminManager Create(IdentityFactoryOptions<AdminManager> options, IOwinContext context)
        {
            var manager = new AdminManager(new UserStore<Admin>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<Admin>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class AdminSignInManager : SignInManager<Admin, string>
    {
        public AdminSignInManager(AdminManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Admin user)
        {
            return user.GenerateUserIdentityAsync((AdminManager)UserManager);
        }

        public static AdminSignInManager Create(IdentityFactoryOptions<AdminSignInManager> options, IOwinContext context)
        {
            return new AdminSignInManager(context.GetUserManager<AdminManager>(), context.Authentication);
        }
    }
}