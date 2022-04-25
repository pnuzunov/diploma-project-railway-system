using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class Ticket : BaseEntity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }

        public string BeginStation { get; set; }
        public string EndStation { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Departure { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Arrival { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime BuyDate { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string TrainName { get; set; }
        public string TrainType { get; set; }
        public string SeatNumbers { get; set; }
        public string SeatType { get; set; }
        public string QRCode { get; set; }
        public int PaymentMethod { get; set; }
    }
}