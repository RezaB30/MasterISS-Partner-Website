using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class GetPartnerSubscriptionListViewModel
    {
        public string DisplayName { get; set; }
        public long SubscriptionId { get; set; }

        [UIHint("ShortDateTimeConverted")]
        [Display(ResourceType = typeof(Localization.Model), Name = "MembershipDate")]
        public DateTime MembershipDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SubscriberNo")]
        public string  SubscriberNo { get; set; }
        public string StateName { get; set; }
        public int  StateValue { get; set; }
    }
}