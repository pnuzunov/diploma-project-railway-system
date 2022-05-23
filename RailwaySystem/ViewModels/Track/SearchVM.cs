using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class SearchVM : BaseIndexVM
    {
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
    }
}