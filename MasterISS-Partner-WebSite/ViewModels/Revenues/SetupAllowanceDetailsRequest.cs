using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Revenues
{
    public class SetupAllowanceDetailsRequest
    {
        public int AllowanceCollectionID { get; set; }
        public PageSettingsByWebService PageInfo { get; set; }

    }
}