using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class OperationHistoryListViewModel
    {
        [Display(Name = "TaskNo", ResourceType = typeof(Localization.Model))]
        public long TaskNo { get; set; }

        [Display(Name = "UserSubMail", ResourceType = typeof(Localization.Model))]
        public string UserSubMail { get; set; }

        [Display(Name = "OperationType", ResourceType = typeof(Localization.Model))]
        public string OperationType { get; set; }

        [Display(Name = "ChangeTime", ResourceType = typeof(Localization.Model))]
        public DateTime ChangeTime { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Localization.Model))]
        public string Description { get; set; }
    }
}