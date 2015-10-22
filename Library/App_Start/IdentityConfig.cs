using Library.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Library
{
    public class MyDbInitializer : DropCreateDatabaseAlways<AdminDbContext>
    {
        protected override void Seed(AdminDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        private void InitializeIdentityForEF(AdminDbContext context)
        {
            var UserManager = new UserManager<Admin>(new UserStore<Admin>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string name = "Admin";
            string password = "123456";
            string test = "test";

            //Create Role Test and User Test
            RoleManager.Create(new IdentityRole(test));
            UserManager.Create(new Admin() { UserName = test });

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }

            //Create User=Admin with password=123456
            var user = new Admin();
            user.UserName = name;
            var adminresult = UserManager.Create(user, password);

            //Add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, name);
            }
        }
    }
}