using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class AdminAddEditModel
    {
        public AdminAddEditModel()
        {
            Publishers = new List<SelectListItem>();
        }

        public int PublisherId { get; set; }

        [Required]
        public string RecordName { get; set; }

        [Required]
        public string ISBN { get; set; }

        public string NumberOfPages { get; set; }

        public string CreationDate { get; set; }

        public string Annotation { get; set; }

        public string RecordDescription { get; set; }

        [Required]
        public string RecordAuthor { get; set; }

        [Required]
        public string PublisherName { get; set; }

        public string PublisherAddress { get; set; }

        public string PublisherNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string PublisherEmail { get; set; }

        public List<SelectListItem> Publishers { get; set; }
    }
}