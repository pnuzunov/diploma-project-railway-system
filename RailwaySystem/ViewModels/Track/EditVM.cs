using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Track
{
    public class EditVM : BaseEditVM
    {
        [DisplayName("Start Station")]
        [Required(ErrorMessage = "This field is required!")]
        public int StartStationId { get; set; }
        [DisplayName("End Station")]
        [Required(ErrorMessage = "This field is required!")]
        public int EndStationId { get; set; }
    }
}