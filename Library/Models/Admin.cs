using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Library.Models
{
    public class Admin : IdentityUser
    {
        public string Name { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}