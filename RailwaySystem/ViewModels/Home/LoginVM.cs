using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Home
{
    public class LoginVM
    {
        [DisplayName("Username")]
        [Required(ErrorMessage = "Please enter a username!")]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter a password!")]
        public string Password { get; set; }
    }
}