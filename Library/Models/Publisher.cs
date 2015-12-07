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

        public string Address { get; set; }

        public string Number { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}