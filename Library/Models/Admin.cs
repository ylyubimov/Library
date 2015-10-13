using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен быть длиннее 6 символов.")]
        public string Password { get; set; }
    }
}