using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class GetCustomerSessionInfoResponseViewModel
    {
        public SessionInfo FirstSessionInfo { get; set; }
        public SessionInfo LastSessionInfo { get; set; }
    }

    public class SessionInfo
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "IsOnline")]
        [UIHint("IsBoolFormat")]
        public bool IsOnline { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SessionId")]
        public string SessionId { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "NASIPAddress")]
        public string NASIPAddress { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "IPAddress")]
        public string IPAddress { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SessionTime")]
        public TimeSpan SessionTime { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SessionStart")]
        [UIHint("DateTimeConverted")]
        public DateTime SessionStart { get; set; }
    }
}