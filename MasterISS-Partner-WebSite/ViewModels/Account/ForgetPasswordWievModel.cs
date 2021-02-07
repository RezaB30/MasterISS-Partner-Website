using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MasterISS_Partner_WebSite.Localization;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class ForgetPasswordWievModel
    {
        [Display(ResourceType = typeof(Model), Name = "ExecutivePhoneNo")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string ExecutivePhoneNo { get; set; }
        [Display(ResourceType = typeof(Model), Name = "TaxNo")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string TaxNo { get; set; }
        [Display(ResourceType = typeof(Model), Name = "Username")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string Username { get; set; }
    }
}