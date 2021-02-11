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
        public IList<string> SelectedRole { get; set; }
        public IList<SelectListItem> AvaibleRole { get; set; }
        public NewUserViewModel()
        {
            SelectedRole = new List<string>();
            AvaibleRole = new List<SelectListItem>();
        }

        [MaxLength(300, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "EmailMaxLenght")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "EmailFormat")]
        [Display(ResourceType = typeof(Localization.Model), Name = "UserEmail")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string UserEmail{ get; set; }

        //public int RoleId { get; set; }

        [MinLength(6, ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "PasswordMinLenght")]
        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UserNameSurname")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string UserNameSurname{ get; set; }

    }
}