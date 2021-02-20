using MasterISS_Partner_WebSite.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class GetBasicAllowDetailViewModel
    {
        public Enums.RevenuesTypeEnum RevenuesTypeEnum { get; set; }
        public PageSettingsByWebService PageInfo { get; set; }
    }
}