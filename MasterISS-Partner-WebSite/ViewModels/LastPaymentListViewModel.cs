using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class LastPaymentListViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "SubcriberName")]
        public string SubcriberName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SubcriberNo")]
        public string SubcriberNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "PaymentDate")]
        public DateTime PaymentDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Amount")]
        public decimal Amount { get; set; }
    }
}