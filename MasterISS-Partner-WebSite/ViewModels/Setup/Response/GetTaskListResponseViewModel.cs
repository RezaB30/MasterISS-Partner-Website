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
    }
}