using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class PartnerSubscriptionFilterViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedName")]
        public string SearchedName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedSubsNo")]
        public string SearchedSubsNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedCustomerState")]
        public int? SearchedCustomerStatus { get; set; }
    }
}