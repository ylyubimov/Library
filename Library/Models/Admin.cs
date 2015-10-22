using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Library.Models
{
    public class Admin : IdentityUser
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен быть длиннее 6 символов.")]
        public string Password { get; set; }

        public string Name { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Admin> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class AdminDbContext : IdentityDbContext<Admin>
    {
        public AdminDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static AdminDbContext Create()
        {
            return new AdminDbContext();
        }
    }
}