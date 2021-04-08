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

        public List<SetupTeamStaffsToMatchedTheTask> SetupTeamStaffsToMatchedTheTask { get; set; }

        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Description")]
        [DataType(DataType.MultilineText)]
        [MaxLength(450, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "RequiredMaxDecription")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public long? StaffId { get; set; }

    }
}