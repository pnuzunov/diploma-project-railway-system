using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Ticket
{
    public class BuyVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TrainName { get; set; }
        public string TrainType { get; set; }
        public int ScheduleId { get; set; }
        public string StartStationName { get; set; }
        public string EndStationName { get; set; }
        public DateTime DepartureDate { get; set; }
        public int Quantity { get; set; }
        public string SeatType { get; set; }
        public decimal Price { get; set; }
        public DateTime BuyDate { get; set; }
    }
}