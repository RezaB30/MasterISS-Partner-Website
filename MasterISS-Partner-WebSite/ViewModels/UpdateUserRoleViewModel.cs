using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class UpdateUserRoleViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "UserEmail")]
        public string UserEmail { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UserNameSurname")]
        public string UserNameSurname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Localization.Model), Name = "RoleId")]
        public int RoleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int UserId { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "PhoneNo")]
        [RegularExpression(@"^((\d{3})(\d{3})(\d{2})(\d{2}))$", ErrorMessageResourceName = "ValidPhoneNumber", ErrorMessageResourceType = typeof(Localization.Validation))]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string PhoneNumber { get; set; }

    }
}