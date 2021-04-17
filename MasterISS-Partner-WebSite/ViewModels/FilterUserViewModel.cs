using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class FilterUserViewModel
    {
        public int? SelectedPermission { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        public string SelectedUsername { get; set; }
    }
}