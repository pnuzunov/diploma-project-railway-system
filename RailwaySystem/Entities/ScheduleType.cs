using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailwaySystem.Entities
{
    public class ScheduleType : BaseEntity
    {
        public string Description { get; set; }
    }
}