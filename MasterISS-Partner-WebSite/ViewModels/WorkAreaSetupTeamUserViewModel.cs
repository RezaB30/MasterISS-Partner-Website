using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class WorkAreaSetupTeamUserViewModel
    {
        public int UserId { get; set; }

        public long? WorkAreaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ProvinceId", ResourceType = typeof(Localization.Model))]
        public long ProvinceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string ProvinceName { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Localization.Model))]
        public long? DistrictId { get; set; }

        public string DistrictName { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Localization.Model))]
        public long? RuralId { get; set; }

        public string RuralName { get; set; }

        [Display(Name = "DistrictId", ResourceType = typeof(Localization.Model))]
        public long? NeigborhoodId { get; set; }

        public string NeigborhoodName { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<SetupTeamUserAddressInfo> SetupTeamUserAddressInfo { get; set; }

    }

}