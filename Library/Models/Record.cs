﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Record
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string RecordName { get; set; }

        public string RecordDescription { get; set; }

        [Required]
        public string AuthorName { get; set; }

        public string NumberOfPages { get; set; }

        public string CreationDate { get; set; }

        public DateTime AdditionDate { get; set; }

        public string Annotation { get; set; }

        public int PublisherId { get; set; }

        public bool Recomended { get; set; }

        [ForeignKey("PublisherId")]
        public virtual Publisher RecordPublisher { get; set; }
    }
}