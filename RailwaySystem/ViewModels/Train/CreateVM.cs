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
        [Required(ErrorMessage = "Please enter a name for the train.")]
        public string Name { get; set; }

        [DisplayName("Train Type")]
        [Required(ErrorMessage = "Please select a train type.")]
        public int TypeId { get; set; }

        [DisplayName("Seats First Class")]
        [Required(ErrorMessage = "Please enter a valid number.")]
        public int SeatsFirstClass { get; set; }

        [DisplayName("Regular Seats")]
        [Required(ErrorMessage = "Please enter a valid number.")]
        public int RegularSeats { get; set; }

    }
}