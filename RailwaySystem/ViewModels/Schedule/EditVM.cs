using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailwaySystem.ViewModels.Schedule
{
    public class EditVM
    {
        public enum EditOptions
        {
            ONLY_THIS_ENTRY = 0,
            BY_DEFINED_PERIOD = 1,
            ALL_MATCHING_ENTRIES = 2
        }

        public int Id { get; set; }
        [DisplayName("Train")]
        public int TrainId { get; set; }

        [DisplayName("Cancel this entry")]
        public bool Cancelled { get; set; }

        public DateTime DepartDate { get; set; }
        [DisplayName("Apply changes to")]
        public EditOptions EditOption { get; set; }

        public DateTime LastDateToApply { get; set; }
    }
}