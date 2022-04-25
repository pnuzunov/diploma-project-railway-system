using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class CreateVM : BaseCreateVM
    {
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Way Stations")]
        public List<int> WayStations { get; set; }
    }
}