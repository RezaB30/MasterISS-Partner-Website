using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup.Response
{
    public class GetCustomerLineDetailsViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "XDSLNo")]
        public string XDSLNo { get; set; }

        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "IsActive")]
        [UIHint("IsBoolFormat")]
        public bool IsActive { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "DowloadNoiseMargin")]
        public string DowloadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UploadNoiseMargin")]
        public string UploadNoiseMargin { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "CurrentDowloadSpeed")]
        public string CurrentDowloadSpeed { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "CurrentUploadSpeed")]
        public string CurrentUploadSpeed { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "DowloadSpeedCapasity")]
        public string DowloadSpeedCapasity { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "UploadSpeedCapasity")]
        public string UploadSpeedCapasity { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "ShelfCardPort")]
        public string ShelfCardPort { get; set; }
    }
}