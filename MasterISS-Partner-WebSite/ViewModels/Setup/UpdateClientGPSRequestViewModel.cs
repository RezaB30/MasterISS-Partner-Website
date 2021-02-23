using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup
{
    public class UpdateClientGPSRequestViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "Latitude")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Latitude { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Longitude")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Longitude { get; set; }
        public long TaskNo { get; set; }
    }
}