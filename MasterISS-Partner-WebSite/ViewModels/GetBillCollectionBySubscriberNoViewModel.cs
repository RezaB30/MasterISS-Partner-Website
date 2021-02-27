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
        public string SubscriberNo { get; set; }
    }
}