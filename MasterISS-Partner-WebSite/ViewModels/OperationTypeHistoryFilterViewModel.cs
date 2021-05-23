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

        [RegularExpression("^(\\d{2}).\\d{2}.(\\d{4})$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string StartDate { get; set; }

        [RegularExpression("^(\\d{2}).\\d{2}.(\\d{4})$", ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "DateFormatErrorReservationDate")]
        public string EndDate { get; set; }
    }
}