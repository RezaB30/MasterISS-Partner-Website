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

        [Display(Name = "AvailablePermission", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int[] AvailableRoles { get; set; }
    }
    public class AvailablePermissionList
    {
        public string PermissionName { get; set; }
        public int PermissionId { get; set; }
        public string IsSelected { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "AvailablePermission", ResourceType = typeof(Localization.Model))]
        public int[] SelectedPermissions { get; set; }

    }
    public class AvailableRoleList
    {
        [Display(Name = "RolePermissionName", ResourceType = typeof(Localization.Model))]
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }
}