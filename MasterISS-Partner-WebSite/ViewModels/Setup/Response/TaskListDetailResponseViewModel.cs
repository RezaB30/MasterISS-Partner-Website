using MasterISS_Partner_WebSite_Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class TaskListDetailResponseViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "CustomerNo")]
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Province")]
        public string Province { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "City")]
        public string City { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "BBK")]
        public string BBK { get; set; }


        [Display(ResourceType = typeof(Localization.Model), Name = "PSTN")]
        public string PSTN { get; set; }

        [UIHint("HasModemFormat")]
        [Display(ResourceType = typeof(Localization.Model), Name = "HasModem")]
        public bool HasModem { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "ModemName")]
        public string ModemName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "XDSLType")]
        public XDSLTypeEnum XDSLType { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "CustomerType")]
        public string CustomerType { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "LastConnectionDate")]
        public DateTime LastConnectionDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Details")]
        public string Details { get; set; }

        public GetCustomerCredentialsResponseViewModel GetCustomerCredentialsResponseViewModel { get; set; }

        public IEnumerable<TaskUpdatesDetailListViewModel> TaskUpdatesDetailList { get; set; }
    }
    public class TaskUpdatesDetailListViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "FaultCodes")]
        public string FaultCodes { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "CreationDate")]
        [UIHint("DateTimeConverted")]
        public DateTime CreationDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "ReservationDate")]
        [UIHint("DateTimeConverted")]
        public DateTime ReservationDate { get; set; }
    }
}