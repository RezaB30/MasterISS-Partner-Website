using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Revenues
{
    public class AllowenceListViewModel
    {
        public long Id { get; set; }

        [Display(Name = "Ispaid", ResourceType = typeof(Localization.Model))]
        public bool Ispaid { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DateTimeConverted")]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "PaymentDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DateTimeConverted")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Total", ResourceType = typeof(Localization.Model))]
        public decimal Total { get; set; }
    }
}