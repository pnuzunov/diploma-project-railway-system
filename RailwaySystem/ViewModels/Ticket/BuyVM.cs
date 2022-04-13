using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwaySystem.ViewModels.Ticket
{
    public class BuyVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TrainName { get; set; }
        public string TrainType { get; set; }
        public Entities.Schedule Schedule { get; set; }
        public Entities.Station StartStation { get; set; }
        public Entities.Station EndStation { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }

        [DisplayName("Number of tickets")]
        [Required(ErrorMessage = "This field is required!")]
        public int Quantity { get; set; }
        [DisplayName("Seat type")]
        [Required(ErrorMessage = "This field is required!")]
        public string SeatType { get; set; }

        public List<Entities.Seat> Seats { get; set; }
        public decimal Price { get; set; }
        public DateTime BuyDate { get; set; }
    }
}