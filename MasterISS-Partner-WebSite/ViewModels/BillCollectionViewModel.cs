using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class BillCollectionViewModel
    {
        [Required]
        [Display(Name = "SubscriberNo", ResourceType = typeof(Localization.Model))]
        public string SubscriberName { get; set; }
        public IEnumerable<BillsViewModel> Bills { get; set; }
    }

    public class BillsViewModel
    {
        public long BillID { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Localization.Model))]
        public decimal Cost { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(Localization.Model))]
        public string IssueDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(Localization.Model))]
        public string DueDate { get; set; }

    }
}