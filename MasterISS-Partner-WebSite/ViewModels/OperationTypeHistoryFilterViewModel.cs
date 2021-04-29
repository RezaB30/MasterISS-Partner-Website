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

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string StartDate { get; set; }

        [RegularExpression("^(3[01]|[12][0-9]|0[1-9]).(1[0-2]|0[1-9]).[0-9]{4} (2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string EndDate { get; set; }
    }
}