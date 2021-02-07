using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class BillCollectionSettingsViewModel
    {
        [Display(Name = "Username", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Username { get; set; }
        [Display(Name = "Password", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Password { get; set; }
    }
}