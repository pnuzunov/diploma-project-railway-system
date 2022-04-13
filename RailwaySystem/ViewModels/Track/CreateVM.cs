using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class CreateVM : BaseCreateVM
    {
        [DisplayName("Station")]
        public List<int> WayStations { get; set; }
        [DisplayName("Price for ticket")]
        public List<decimal> PriceChanges { get; set; }
        [DisplayName("Minutes to arrive")]
        public List<int> MinutesToArrive { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public decimal StandardTicketPrice { get; set; }
    }
}