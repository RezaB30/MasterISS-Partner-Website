using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class GetTaskListResponseViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "TaskNo")]
        public long TaskNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SetupAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "CustomerPhoneNo")]
        public string CustomerPhoneNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "ContactName")]
        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskIssueDate")]
        [UIHint("DateTimeConverted")]
        public DateTime TaskIssueDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskType")]
        public string TaskType { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskStatus")]
        public string TaskStatus { get; set; }

        [UIHint("DateTimeConverted")]
        [Display(ResourceType = typeof(Localization.Model), Name = "ReservationDate")]
        public DateTime ReservationDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "XDSLNo")]
        public string XDSLNo { get; set; }
    }
}