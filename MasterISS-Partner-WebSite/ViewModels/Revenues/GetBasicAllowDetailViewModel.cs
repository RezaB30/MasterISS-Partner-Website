using MasterISS_Partner_WebSite.ViewModels.Revenues;
using MasterISS_Partner_WebSite_Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class GetBasicAllowDetailViewModel
    {
        public RevenuesTypeEnum RevenuesTypeEnum { get; set; }
        public PageSettingsByWebService PageInfo { get; set; }
    }
}