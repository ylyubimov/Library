using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class PublisherEditModel
    {
        public PublisherEditModel() { }

        public PublisherEditModel(Publisher publisher)
        {
            PublisherName = publisher.PublisherName;
            Address = publisher.Address;
            Number = publisher.Number;
            Email = publisher.Email;
        }

        [Required]
        public string PublisherName { get; set; }

        public string Address { get; set; }

        public string Number { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}