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
        public string SubscriberNo { get; set; }
        public IEnumerable<BillsViewModel> Bills { get; set; }
        //public string SelectedBills { get; set; }
    }

    public class BillsViewModel
    {
        public string BillID { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Localization.Model))]
        public string Cost { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(Localization.Model))]
        public string IssueDate { get; set; }

        [Display(Name = "DueDate", ResourceType = typeof(Localization.Model))]
        public string DueDate { get; set; }

        public bool IsPayBill { get; set; }
    }
}