using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Station
{
    public class EditVM : BaseEditVM
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "This field is required!")]
        public string Name { get; set; }
    }
}