using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class NewUserViewModel
    {
        public List<SelectListItem> UserRoleList { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "PhoneNo")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string PhoneNumber { get; set; }

    }
}