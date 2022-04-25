using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Schedule
{
    public class SwsVM
    {
        public string StationName { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
    }
}