using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Train
{
    public class CreateVM : BaseCreateVM
    {

        [DisplayName("Name")]
        [Required(ErrorMessage = "This field is required!")]
        public string Name { get; set; }

        [DisplayName("Train Type")]
        [Required(ErrorMessage = "This field is required!")]
        public short Type { get; set; }
    }
}