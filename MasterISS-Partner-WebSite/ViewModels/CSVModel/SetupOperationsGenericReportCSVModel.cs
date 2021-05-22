using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupOperationsGenericReportCSVModel
    {
        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "TaskIssueDate")]
        public string TaskIssueDate { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "NameSurname")]
        public string NameSurname { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "TaskType")]
        public string TaskType { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "Stage")]
        public string Stage { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "CustomerStatus")]
        public string CustomerStatus { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "TaskStatus")]
        public string TaskStatus { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "LastRendezvousDate")]
        public string LastRendezvousDate { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "SetupTeamStaff")]
        public string SetupTeamStaff { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "Area")]
        public string Area { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "CompletionDate")]
        public string CompletionDate { get; set; }

        [Display(ResourceType = typeof(Localization.TaskCSVReport), Name = "Description")]
        public string Description { get; set; }

    }
}