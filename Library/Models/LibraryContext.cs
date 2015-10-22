using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class LibraryContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public LibraryContext()
            : base("LibraryConnection")
        {
        }
    }
}