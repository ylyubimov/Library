using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }

        [Required]
        public string PublisherName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"[0-9]+(\-[0-9]+)*")]
        public string Number { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set;}
        // public List<Record> Records { get; set; }
    }
}