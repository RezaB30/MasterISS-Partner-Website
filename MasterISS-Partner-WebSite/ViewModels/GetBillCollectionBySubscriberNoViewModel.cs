using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class GetBillCollectionBySubscriberNoViewModel
    {
        [Display(Name = "RolePermissionName", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        //[MinLength(10, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "SubscriberNoValid")]
        //[MaxLength(10, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "SubscriberNoValid")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "OnlyNumeric")]
        public string SubscriberNo { get; set; }
    }
}