using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Publisher
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }   
        public string Email { get; set;}
        // public List<Record> Records { get; set; }
    }
}