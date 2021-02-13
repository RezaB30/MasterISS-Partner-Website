using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class CreditReportViewModel
    {
        [Display(Name = "TotalCredit", ResourceType = typeof(Localization.Model))]
        public decimal Total { get; set; }
        public List<CreditReportDetailsViewModel> CreditReportDetailsViewModel { get; set; }
    }

    public class CreditReportDetailsViewModel
    {

        [Display(Name = "Amount", ResourceType = typeof(Localization.Model))]
        public decimal Amount { get; set; }


        [Display(Name = "Date", ResourceType = typeof(Localization.Model))]
        public string Date { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Localization.Model))]
        public string Details { get; set; }
    }
}
