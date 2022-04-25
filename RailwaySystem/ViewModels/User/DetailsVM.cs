using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace RailwaySystem.ViewModels.User
{
    public class DetailsVM
    {
        public int Id { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [DisplayName("Total Credit")]
        public decimal Credit { get; set; }
    }
}