using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class PartnerSubscriptionFilterViewModel
    {
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(Localization.Model), Name = "CustomerMembershipStartDate")]
        public string CustomerMembershipStartDate { get; set; }

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        [Display(ResourceType = typeof(Localization.Model), Name = "CustomerMembershipEndDate")]
        public string CustomerMembershipEndDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedName")]
        public string SearchedName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SearchedSubsNo")]
        public long? SearchedSubsNo { get; set; }
    }
}