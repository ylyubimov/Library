using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Library.Models
{
    public class ApplicationUser : IdentityUser
    {
    }

    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<LibraryContext>(new DataBaseInitializer());
        }

        public static LibraryContext Create()
        {
            return new LibraryContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Admin>()
            .Map(m =>
            {
                m.Requires("Person_Type").HasValue("Person");
            });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}