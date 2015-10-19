using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Record = new Record();
        }

        public int PublisherId { get; set; }
        public Record Record { get; set; }
        public List<SelectListItem> Publishers { get; set; }
    }
}