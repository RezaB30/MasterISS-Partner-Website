using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup
{
    public class GetTaskListRequestViewModel
    {
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(Localization.Model), Name = "TaskListStartDate")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string TaskListStartDate { get; set; }

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(Localization.Model), Name = "TaskListEndDate")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string TaskListEndDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedName")]
        public string SearchedName { get; set; }
    }
}