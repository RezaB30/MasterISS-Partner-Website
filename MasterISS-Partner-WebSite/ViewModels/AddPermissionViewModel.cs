using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class AddPermissionViewModel
    {
        [Display(Name = "RolePermissionName", ResourceType = typeof(Localization.Model))]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public IList<string> SelectedRole { get; set; }
        public IEnumerable<SelectListItem> AvaibleRole { get; set; }
        public AddPermissionViewModel()
        {
            SelectedRole = new List<string>();
            AvaibleRole = new List<SelectListItem>();
        }
    }
}