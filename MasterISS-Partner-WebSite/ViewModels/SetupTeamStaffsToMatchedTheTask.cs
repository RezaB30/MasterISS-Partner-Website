using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamStaffsToMatchedTheTask
    {
        public long SetupTeamStaffId { get; set; }

        [Display(Name = "SetupTeamStaffName", ResourceType = typeof(Localization.Model))]
        public string SetupTeamStaffName { get; set; }

        [Display(Name = "SetupTeamStaffWorkAreas", ResourceType = typeof(Localization.Model))]
        public List<SetupTeamUserAddressInfo> SetupTeamStaffWorkAreas { get; set; }

        public string SelectedDate { get; set; }

        public string SelectedTime { get; set; }

    }
}