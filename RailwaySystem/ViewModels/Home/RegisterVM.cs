using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Home
{
    public class RegisterVM
    {
        [DisplayName("Username")]
        [Required]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }

        [DisplayName("Repeat Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        [Required]
        public string RepeatPassword { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Email Address")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        [Required]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Required]
        public string Phone { get; set; }
    }
}