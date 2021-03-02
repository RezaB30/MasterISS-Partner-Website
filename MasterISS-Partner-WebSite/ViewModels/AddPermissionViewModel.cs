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
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string RoleName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int[] AvailableRoles { get; set; }
        //public IList<int> SelectedRole { get; set; }
        //public IEnumerable<SelectListItem> AvaibleRole { get; set; }
        //public AddPermissionViewModel()
        //{
        //    SelectedRole = new List<int>();
        //    AvaibleRole = new List<SelectListItem>();
        //}
    }
    public class AvailableRoleList
    {
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }
}