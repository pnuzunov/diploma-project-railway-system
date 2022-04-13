using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class EditVM : BaseEditVM
    {
        [DisplayName("Way Stations")]
        public List<int> WayStations { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public decimal StandardTicketPrice { get; set; }
    }
}