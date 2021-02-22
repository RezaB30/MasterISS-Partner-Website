using MasterISS_Partner_WebSite.CustomerSetupServiceReference;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class SetupServiceWrapper
    {
        private readonly string Username;

        private readonly string Rand;

        private readonly string Culture;

        private readonly string KeyFragment;

        private readonly string Password;
        private CustomerSetupServiceClient Client { get; set; }

        public SetupServiceWrapper()
        {
            var claimInfo = new ClaimInfo();
            Username = claimInfo.PartnerSetupServiceUser();
            Culture = CultureInfo.CurrentCulture.ToString();
            KeyFragment = new CustomerSetupServiceClient().GetKeyFragment(claimInfo.PartnerSetupServiceUser());
            Rand = Guid.NewGuid().ToString("N");
            Password = claimInfo.PartnerSetupServiceHash();
            Client = new CustomerSetupServiceClient();
        }
    }
}