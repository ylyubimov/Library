using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Record
    {
        public int RecordId { get; set; }
        public string RecordName { get; set; }
        public string RecordDescription { get; set; }

        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public virtual Publisher Author { get; set; } 
    }
}