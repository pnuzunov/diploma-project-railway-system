using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class ScheduleCancellation : BaseEntity
    {
        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CancellationDate { get; set; }
    }
}