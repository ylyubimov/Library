using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class AdminLoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен быть длиннее 6 символов.")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}