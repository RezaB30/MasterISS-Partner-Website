using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SMSApiSettingsViewModel
    {
        [Display(Name = "Username", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "UsernameNull", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Username { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceName = "PasswordNull", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "sourceAddrRequired", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string SourceAddr { get; set; }

        [Required(ErrorMessageResourceName = "smsValidForRequired", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string SmsValidFor { get; set; }
    }
}