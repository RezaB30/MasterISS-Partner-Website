using MasterISS_Partner_WebSite_Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup
{
    public class AddTaskStatusUpdateViewModel
    {
        public long? TaskNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "FaultCodes")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public FaultCodeEnum FaultCodes { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Description")]
        [DataType(DataType.MultilineText)]
        [MaxLength(450, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "RequiredMaxDecription")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }
        public List<DateTime> StaffCalendar { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public long? StaffId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public DateTime? SelectedDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public TimeSpan? SelectedTime { get; set; }
    }
}