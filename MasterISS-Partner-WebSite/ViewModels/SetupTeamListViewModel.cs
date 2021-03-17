using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamListViewModel
    {
        public long Id { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UserNameSurname")]
        public string UserDisplayName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "WorkingStatus")]
        [UIHint("WorkingTypeFormat")]
        public bool WorkingStatus { get; set; }

        public IEnumerable<SetupTeamUserAddressInfo> SetupTeamUserAddressInfo { get; set; }
    }

    public class SetupTeamUserAddressInfo
    {
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string RuralName { get; set; }
        public string NeigborhoodName { get; set; }
    }
}