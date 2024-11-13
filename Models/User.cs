using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required (ErrorMessage = "enter username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "please enter username")]
        public string Password { get; set; }

    }
}