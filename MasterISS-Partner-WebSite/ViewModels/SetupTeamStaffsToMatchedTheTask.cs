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

        public string SelectedDate { get; set; }

        public string SelectedTime { get; set; }

    }
}