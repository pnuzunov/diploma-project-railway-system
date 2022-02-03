using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Train
{
    public class EditVM : BaseEditVM
    {
        [DisplayName("Name: ")]
        [Required(ErrorMessage = "This field is Required!")]
        public string Name { get; set; }

        [DisplayName("Type: ")]
        [Required(ErrorMessage = "This field is Required!")]
        public int Type { get; set; }
    }
}