﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamStaffsToMatchedTheTask
    {
        public long SetupTeamStaffId { get; set; }
        public string SetupTeamStaffName { get; set; }
        public List<SetupTeamUserAddressInfo> SetupTeamStaffWorkAreas { get; set; }


        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public DateTime? SelectedDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public TimeSpan? SelectedTime { get; set; }

        public SelectList SetupTeamStaffWorkDays { get; set; }
    }
}