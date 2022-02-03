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
        public User user { get; set; }
        public string BeginStation { get; set; }
        public string EndStation { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Departure { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime BuyDate { get; set; }
        public decimal Price { get; set; }
        public string TrainName { get; set; }
        public string TrainType { get; set; }
        public int SeatNumber { get; set; }
        public string QRCode { get; set; }
    }
}