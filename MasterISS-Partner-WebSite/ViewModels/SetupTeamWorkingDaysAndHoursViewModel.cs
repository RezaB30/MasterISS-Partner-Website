using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamWorkingDaysAndHoursViewModel
    {
        public long UserId { get; set; }
        [Display(ResourceType = typeof(Localization.Model), Name = "StartingTimeOfWork")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "TimeFormat")]
        public string WorkingStartTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Model), Name = "EndTimeOfWork")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "TimeFormat")]
        public string WorkingEndTime { get; set; }

        public string ContectName { get; set; }
        public List<AvailableWorkingDays> AvailableWorkingDays { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "WorkingDays")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int[] SelectedDays { get; set; }
    }

    public class AvailableWorkingDays
    {
        public string DayName { get; set; }
        public int DayId { get; set; }
        public bool IsSelected { get; set; }
        public int[] SelectedDays { get; set; }
    }
}