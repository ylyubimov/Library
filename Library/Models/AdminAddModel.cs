using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class AdminAddModel
    {
        public AdminAddModel()
        {
            Publishers = new List<SelectListItem>();
        }

        public AdminAddModel(List<SelectListItem> publishers)
        {
            Publishers = publishers;
        }

        public int PublisherId { get; set; }
        public Record Record { get; set; }
        public List<SelectListItem> Publishers { get; set; }
    }
}