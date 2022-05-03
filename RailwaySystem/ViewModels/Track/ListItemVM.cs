using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class ListItemVM
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public List<WayStationVM> WayStations { get; set; }
    }
}