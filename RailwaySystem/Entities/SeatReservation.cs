using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class SeatReservation : BaseEntity
    {
        public int SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat Seat { get; set; }
        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Departure { get; set; }
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
    }
}