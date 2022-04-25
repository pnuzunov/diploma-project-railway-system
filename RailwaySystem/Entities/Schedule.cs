using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RailwaySystem.Entities
{
    public class Schedule : BaseEntity
    {
        public int TrainId { get; set; }
        public int TrackId { get; set; }    

        [ForeignKey("TrainId")]
        public Train WhichTrain { get; set; }
        [ForeignKey("TrackId")]
        public Track WhichTrack { get; set; }
        public decimal PricePerTicket { get; set; }
        public int ScheduleModeId { get; set; }

        public bool Cancelled { get; set; }
    }
}