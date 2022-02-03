using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RailwaySystem.ViewModels.Schedule
{
    public class EditVM : BaseEditVM
    {
        [DisplayName("Train")]
        [Required(ErrorMessage = "This field is required!")]
        public int TrainId { get; set; }
        [DisplayName("Route")]
        [Required(ErrorMessage = "This field is required!")]
        public int TrackId { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Departs"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime Departure { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Arrives"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime Arrival { get; set; }
    }
}