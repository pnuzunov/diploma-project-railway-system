using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Schedule
{
    public class ListItemVM
    {
        public Entities.Schedule Schedule { get; set; }
        public string Route { get; set; }
        public List<SwsVM> ScheduledWayStations { get; set; }
        public string Train { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
    }
}