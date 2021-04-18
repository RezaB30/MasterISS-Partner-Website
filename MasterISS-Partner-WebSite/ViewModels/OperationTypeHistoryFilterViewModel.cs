using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class OperationTypeHistoryFilterViewModel
    {
        [Display(Name = "OperationType", ResourceType = typeof(Localization.Model))]
        public short? OperationType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}