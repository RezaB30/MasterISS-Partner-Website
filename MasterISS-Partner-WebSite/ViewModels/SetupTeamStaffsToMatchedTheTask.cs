using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamStaffsToMatchedTheTask
    {
        public long SetupTeamStaffId { get; set; }
        public string SetupTeamStaffName { get; set; }
        public List<SetupTeamUserAddressInfo> SetupTeamStaffWorkAreas { get; set; }
    }
}