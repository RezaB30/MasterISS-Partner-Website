using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class UserListViewModel
    {
        public long UserId { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        public string UserSubMail { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UserNameSurname")]
        public string NameSurname { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "IsEnabled")]
        public bool IsEnabled { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "RoleName")]
        public string RoleName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        public bool ısSetupTeam { get; set; }
        public IEnumerable<SetupTeamUserAddressInfo> SetupTeamUserAddressInfo { get; set; }

    }
}