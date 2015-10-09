using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class AdminViewModel
    {
        public AdminViewModel() { }

        public AdminViewModel(Record newRecord, Publisher newPublisher) {
            publisher = newPublisher;
            record = newRecord;
        }

        public Record record { set; get; }
        public Publisher publisher { set; get; }      
    }
}