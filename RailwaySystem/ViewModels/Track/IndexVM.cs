using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class IndexVM
    {
        public Entities.Station StartStation { get; set; }
        public Entities.Station EndStation { get; set; }
    }
}