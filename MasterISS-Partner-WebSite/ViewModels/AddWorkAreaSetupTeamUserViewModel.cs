using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class AddWorkAreaSetupTeamUserViewModel
    {
        public int UserId { get; set; }

        public NameValuePair Provinces { get; set; }
    }

    public class NameValuePair
    {
        public long? Value { get; set; }
        public string Name { get; set; }
    }
}