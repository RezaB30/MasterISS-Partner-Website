using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class ListWorkAreaSetupTeamUserViewModel
    {
        public long WorkAreaId { get; set; }
        public string UserNameSurname { get; set; }
        public SetupTeamUserAddressInfo SetupTeamUserAddressInfo { get; set; }
    }
}