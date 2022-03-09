using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class Seat : BaseEntity
    {
        public bool IsFirstClass { get; set; }
        public int SeatNumber { get; set; }
        public int TrainId { get; set; }
        [ForeignKey("TrainId ")]
        public Train Train { get; set; }
    }
}