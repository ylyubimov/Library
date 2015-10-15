using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class AdminEditModel
    {
        public AdminEditModel() { }

        public AdminEditModel(Record newRecord, Publisher newPublisher) {
            publisher = newPublisher;
            record = newRecord;
        }

        public Record record { set; get; }
        public Publisher publisher { set; get; }      
    }
}