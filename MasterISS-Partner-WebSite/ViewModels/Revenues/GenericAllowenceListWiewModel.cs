using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Revenues
{
    public class GenericAllowenceListWiewModel
    {
        public SaleGenericAllowancesViewModel SaleGenericAllowances { get; set; }

        public SetupGenericAllowancesViewModel SetupGenericAllowances { get; set; }
    }

    public class SetupGenericAllowancesViewModel
    {
        [Display(Name = "Allowance", ResourceType = typeof(Localization.Model))]
        public decimal? Allowance { get; set; }
        public AllowanceState AllowanceState { get; set; }

        [Display(Name = "CompletionDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DateTimeConverted")]
        public DateTime? CompletionDate { get; set; }

        [Display(Name = "IssueDate", ResourceType = typeof(Localization.Model))]
        [UIHint("DateTimeConverted")]
        public DateTime? IssueDate { get; set; }
        public State SetupState { get; set; }

        [Display(Name = "SubscriptionNo", ResourceType = typeof(Localization.Model))]
        public string SubscriptionNo { get; set; }

    }

    public class SaleGenericAllowancesViewModel
    {
        [Display(Name = "Allowance", ResourceType = typeof(Localization.Model))]
        public decimal? Allowance { get; set; }

        [UIHint("DateTimeConverted")]
        [Display(Name = "MembershipDate", ResourceType = typeof(Localization.Model))]
        public DateTime? MembershipDate { get; set; }

        [Display(Name = "SubscriptionNo", ResourceType = typeof(Localization.Model))]
        public string SubscriptionNo { get; set; }

        public AllowanceState AllowanceState { get; set; }
        public State SaleState { get; set; }
    }

    public class State
    {
        [Display(Name = "State", ResourceType = typeof(Localization.Model))]
        public string Name { get; set; }

        public int Value { get; set; }
    }

    public class AllowanceState
    {
        [Display(Name = "AllowanceState", ResourceType = typeof(Localization.Model))]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}