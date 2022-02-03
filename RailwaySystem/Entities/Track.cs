using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RailwaySystem.Entities
{
    public class Track : BaseEntity
    {

        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        [ForeignKey("StartStationId")]
        public Station StartStation { get; set; }
        [ForeignKey("EndStationId")]
        public Station EndStation { get; set; }
    }
}