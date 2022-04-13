using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RailwaySystem.Entities
{
    public class ScheduledWayStation : BaseEntity
    {
        public int WayStationId { get; set; }
        [ForeignKey("WayStationId")]
        public WayStation WayStation { get; set; }
        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Arrival { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Departure { get; set; }
    }
}