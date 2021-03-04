using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class BillCollectionViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "SubscriberName", ResourceType = typeof(Localization.Model))]
        public string SubscriberName { get; set; }
        public IEnumerable<BillsViewModel> Bills { get; set; }
    }

    public class BillsViewModel
    {
        public long BillID { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Localization.Model))]
        public decimal Cost { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DatetimeFormat")]
        public string IssueDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DatetimeFormat")]
        public string DueDate { get; set; }

    }
}