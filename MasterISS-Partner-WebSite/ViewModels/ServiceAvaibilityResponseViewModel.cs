using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class ServiceAvaibilityResponseViewModel
    {
        [Display(Name = "BBKAddress", ResourceType = typeof(Localization.Model))]
        public string Address { get; set; }

        [Display(Name = "BBK", ResourceType = typeof(Localization.Model))]
        public string BBK { get; set; }

        public ADSL ADSL { get; set; }

        public VDSL VDSL { get; set; }

        public FIBER FIBER { get; set; }
    }

    public class FIBER
    {
        [Display(Name = "Distance", ResourceType = typeof(Localization.Model))]
        public int? FiberDistance { get; set; }

        [Display(Name = "PortState", ResourceType = typeof(Localization.Model))]
        public string FiberPortState { get; set; }

        [Display(Name = "Speed", ResourceType = typeof(Localization.Model))]
        public int? FiberSpeed { get; set; }

        [Display(Name = "VUID", ResourceType = typeof(Localization.Model))]
        public string FiberVUID { get; set; }

        [Display(Name = "HasInfrastructure", ResourceType = typeof(Localization.Model))]
        public bool HasInfrastructureFiber { get; set; }
    }

    public class VDSL
    {
        [Display(Name = "Distance", ResourceType = typeof(Localization.Model))]
        public int? VdslDistance { get; set; }

        [Display(Name = "PortState", ResourceType = typeof(Localization.Model))]
        public string VdslPortState { get; set; }

        [Display(Name = "Speed", ResourceType = typeof(Localization.Model))]
        public int? VdslSpeed { get; set; }

        [Display(Name = "VUID", ResourceType = typeof(Localization.Model))]
        public string VdslVUID { get; set; }

        [Display(Name = "HasInfrastructure", ResourceType = typeof(Localization.Model))]
        public bool HasInfrastructureVdsl { get; set; }
    }

    public class ADSL
    {
        [Display(Name = "Distance", ResourceType = typeof(Localization.Model))]
        public int? AdslDistance { get; set; }

        [Display(Name = "PortState", ResourceType = typeof(Localization.Model))]
        public string AdslPortState { get; set; }

        [Display(Name = "Speed", ResourceType = typeof(Localization.Model))]
        public int? AdslSpeed { get; set; }

        [Display(Name = "VUID", ResourceType = typeof(Localization.Model))]
        public string AdslVUID { get; set; }

        [Display(Name = "HasInfrastructure", ResourceType = typeof(Localization.Model))]
        public bool HasInfrastructureAdsl { get; set; }
    }
    
}