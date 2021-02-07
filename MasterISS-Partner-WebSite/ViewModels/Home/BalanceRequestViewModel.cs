using MasterISS_Partner_WebSite.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class BalanceRequestViewModel
    {
        [Display(Name = "BalanceAmount", ResourceType = typeof(Localization.Model))]
        public decimal BalanceAmount { get; set; }

        public List<SelectListItem> BalanceAmountList { get; set; }

        public HttpPostedFileBase[] Files { get; set; }

        public int ID { get; set; }

        public int DealerID { get; set; }

        public string Datetime { get; set; }

        public string DateOrder { get; set; }

        [Display(Name = "Message", ResourceType = typeof(Localization.Model))]
        public string Message { get; set; }

        [Display(Name = "State", ResourceType = typeof(Localization.Model))]
        public BalanceRequestTypesEnum IsConfirm { get; set; }

        public decimal MoreBalanceAmount { get; set; }
    }
}