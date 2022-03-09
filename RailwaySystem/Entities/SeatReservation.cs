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
    }
}