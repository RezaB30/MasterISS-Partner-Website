using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Partner_WebSite.Localization;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class RegisterViewModel
    {
        [Display(ResourceType = typeof(Model), Name = "DealerTitle")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string DealerTitle { get; set; }
        [Display(ResourceType = typeof(Model), Name = "ExecutiveName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string ExecutiveName { get; set; }
        [Display(ResourceType = typeof(Model), Name = "ExecutiveSurname")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string ExecutiveSurname { get; set; }
        [Display(ResourceType = typeof(Model), Name = "Email")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string Email { get; set; }
        [Display(ResourceType = typeof(Model), Name = "ExecutivePhoneNo")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string ExecutivePhoneNo { get; set; }
        [Display(ResourceType = typeof(Model), Name = "TaxNo")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string TaxNo { get; set; }
        [Display(ResourceType = typeof(Model), Name = "Address")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string Address { get; set; }
        public bool IsActive { get; set; }
        [Display(ResourceType = typeof(Model), Name = "RegisterDatetime")]
        public string RegisterDatetime { get; set; }
        [Display(ResourceType = typeof(Model), Name = "Province")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string Province { get; set; }
        public IEnumerable<SelectListItem> ProvinceList { get; set; }
        [Display(ResourceType = typeof(Model), Name = "District")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string District { get; set; }
        public IEnumerable<SelectListItem> DistrictList { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public string CaptchaCode { get; set; }
        [Display(ResourceType = typeof(Model), Name = "CustomerTCK")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Validation))]
        public long ExecutiveTCKNo { get; set; }
    }
}