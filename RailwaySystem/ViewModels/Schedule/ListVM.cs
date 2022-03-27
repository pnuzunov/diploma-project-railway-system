using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwaySystem.ViewModels.Schedule
{
    public class ListVM
    {
        [Required(ErrorMessage = "Please select a station.")]
        public int StartStationId { get; set; }
        [Required(ErrorMessage = "Please select a station.")]
        public int EndStationId { get; set; }
        [Required(ErrorMessage = "Please select the date of departure.")]
        public DateTime DepartureDate { get; set; }
    }
}