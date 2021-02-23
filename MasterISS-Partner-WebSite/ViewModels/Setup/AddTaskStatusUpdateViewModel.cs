using MasterISS_Partner_WebSite.Enums;
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

        [Display(ResourceType = typeof(Localization.Model), Name = "ReservationDate")]
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string ReservationDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Description")]
        [DataType(DataType.MultilineText)]
        [MaxLength(450, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "RequiredMaxDecription")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Description { get; set; }

    }
}