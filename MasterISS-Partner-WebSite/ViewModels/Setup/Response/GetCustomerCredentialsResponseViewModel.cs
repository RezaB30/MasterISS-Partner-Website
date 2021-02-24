using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class GetCustomerCredentialsResponseViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        public string Password { get; set; }
    }
}