using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class UserSignInViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "DealerCode")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        public string DealerCode { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Localization.Validation))]
        //public string CaptchaCode { get; set; }
    }
}