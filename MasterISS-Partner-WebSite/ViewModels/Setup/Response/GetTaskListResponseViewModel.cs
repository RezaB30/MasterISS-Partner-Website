using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class GetTaskListResponseViewModel
    {
        public string BBK { get; set; }
        [Display(ResourceType = typeof(Localization.Model), Name = "TaskNo")]
        public long TaskNo { get; set; }

        public bool IsCorfirmation { get; set; }

        public string[] AddressLatitudeandLongitude { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "ContactName")]
        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskIssueDate")]
        [UIHint("DateTimeConverted")]
        public DateTime TaskIssueDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskType")]
        public string TaskType { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "TaskStatus")]
        public string TaskStatus { get; set; }

        public short? TaskStatusByControl { get; set; }
        public string SubscriberNo { get; set; }

        [UIHint("DateTimeConverted")]
        [Display(ResourceType = typeof(Localization.Model), Name = "ReservationDate")]
        public DateTime ReservationDate { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "XDSLNo")]
        public string XDSLNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SetupAddress")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "FaultCodes")]
        public string FaultCodesDisplayText { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "RendezvousTeam")]
        public string RendezvousTeamStaffName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Description")]
        public string SetupStaffEnteredMessage { get; set; }

        public int PartnerId { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "SetupTeam")]
        public string SetupTeamStaffName { get; set; }
    }
}