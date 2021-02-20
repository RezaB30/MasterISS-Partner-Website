using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Revenues
{
    public class GetBasicAllowenceDetailsResponseList
    {
        public int AllowanceStateID { get; set; }
        
        [Display(Name = "AllowanceStateName", ResourceType = typeof(Localization.Model))]
        public string AllowanceStateName { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Localization.Model))]
        public decimal Price { get; set; }
    }
}