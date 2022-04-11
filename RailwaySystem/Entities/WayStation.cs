using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class WayStation : BaseEntity
    {
        public int TrackId { get; set; }
        [ForeignKey("TrackId")]
        public Track Track { get; set; }
        public int StationId { get; set; }
        [ForeignKey("StationId")]
        public Station Station { get; set; }
        public int ConsecutiveNumber { get; set; }
    }
}