using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Record
    {
        public int RecordId { get; set; }
        public string RecordName { get; set; }
        public Publisher Author { get; set; }
    }
}