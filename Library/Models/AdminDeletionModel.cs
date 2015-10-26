using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class AdminDeletionModel
    {
        public AdminDeletionModel()
        {
        }

        public AdminDeletionModel(Record _record, Publisher _publisher)
        {
            record = _record;
            publisher = _publisher;
        }

        public Record record { get; set; }

        public Publisher publisher { get; set; }
    }
}