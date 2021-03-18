using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Account
{
    public class AdminSignInViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }
    }
}