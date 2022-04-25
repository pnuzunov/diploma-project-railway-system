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

        [DisplayName("City")]
        [Required(ErrorMessage = "This field is required!")]
        public int CityId { get; set; }

        [DisplayName("Latitude")]
        [Required(ErrorMessage = "This field is required!")]
        public decimal Latitude { get; set; }
        [DisplayName("Longitude")]
        [Required(ErrorMessage = "This field is required!")]
        public decimal Longitude { get; set; }
    }
}