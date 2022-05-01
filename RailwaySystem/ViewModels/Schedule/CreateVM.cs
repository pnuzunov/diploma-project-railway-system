using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwaySystem.ViewModels.Schedule
{
    public class CreateVM : BaseCreateVM
    {
        [DisplayName("Train")]
        [Required(ErrorMessage = "This field is required!")]
        public int TrainId { get; set; }

        [DisplayName("Route")]
        [Required(ErrorMessage = "This field is required!")]
        public int TrackId { get; set; }

        //[DisplayName("Departs"), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "This field is required!")]
        [DisplayName("Departure date")]
        public DateTime DepartDate { get; set; }

        //[DisplayName("Arrives"), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "This field is required!")]
        //public DateTime Arrival { get; set; }

        public List<Entities.WayStation> WayStations { get; set; }

        public List<DateTime> Departures { get; set; }

        public List<DateTime> Arrivals { get; set; }

        [DisplayName("Create entries until")]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime LastDateToCreate { get; set; }

        [DisplayName("Schedule Type")]
        [Required(ErrorMessage = "This field is required!")]
        public int ScheduleMode { get; set; }

        [DisplayName("Price per ticket")]
        [Required(ErrorMessage = "This field is required!")]
        public decimal PricePerTicket { get; set; }
    }
}