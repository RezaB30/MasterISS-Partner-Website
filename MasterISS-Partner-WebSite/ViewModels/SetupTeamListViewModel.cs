using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SetupTeamListViewModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UserNameSurname")]
        public string UserDisplayName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "WorkingStatus")]
        [UIHint("WorkingTypeFormat")]
        public bool WorkingStatus { get; set; }
    }
}