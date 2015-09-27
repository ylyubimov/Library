using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class LibraryContext : DbContext
    {
        public List<Record> Books { get; set; }
        public List<Publisher> Authors { get; set; }  
    }
}