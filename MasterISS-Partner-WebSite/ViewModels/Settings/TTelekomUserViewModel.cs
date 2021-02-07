using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class TTelekomUserViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "Username", ResourceType = typeof(Localization.Model))]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "Password", ResourceType = typeof(Localization.Model))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "CustomerCode", ResourceType = typeof(Localization.Model))]
        public long CustomerCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "Domain", ResourceType = typeof(Localization.Model))]
        public string Domain { get; set; }
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ISPCode", ResourceType = typeof(Localization.Model))]

        public int ISPCode { get; set; }
    }
}