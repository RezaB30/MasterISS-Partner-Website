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
        public string RoleName { get; set; }
        [Required]
        public IList<string> SelectedRole { get; set; }
        public IEnumerable<SelectListItem> AvaibleRole { get; set; }
        public AddPermissionViewModel()
        {
            SelectedRole = new List<string>();
            AvaibleRole = new List<SelectListItem>();
        }
    }
}