using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class AddressInfoViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ProvinceId", ResourceType = typeof(Localization.Model))]
        public long? ProvinceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "DistrictId", ResourceType = typeof(Localization.Model))]
        public long? DistrictId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "RuralRegionsId", ResourceType = typeof(Localization.Model))]
        public long? RuralRegionsId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "NeighborhoodId", ResourceType = typeof(Localization.Model))]
        public long? NeighborhoodId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "StreetId", ResourceType = typeof(Localization.Model))]
        public long? StreetId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "BuildingId", ResourceType = typeof(Localization.Model))]
        public long? BuildingId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "ApartmentId", ResourceType = typeof(Localization.Model))]
        public long? ApartmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        [Display(Name = "PostalCode", ResourceType = typeof(Localization.Model))]
        public int? PostalCode { get; set; }

        [Display(Name = "Floor", ResourceType = typeof(Localization.Model))]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public string Floor { get; set; }

        public NewCustomerAddressInfoRequest NewCustomerAddressInfoRequest { get; set; }
    }

    public class NewCustomerAddressInfoRequest
    {

        public long AddressNo { get; set; }

        public string AddressText { get; set; }

        public long ApartmentId{ get; set; }

        public string ApartmentNo { get; set; }

        public long DistrictId{ get; set; }

        public string DistrictName { get; set; }

        public long DoorId { get; set; }

        public string DoorNo { get; set; }

        public long NeighbourhoodID { get; set; }

        public string NeighbourhoodName { get; set; }

        public long ProvinceId{ get; set; }

        public string ProvinceName { get; set; }

        public long RuralCode { get; set; }

        public long StreetId{ get; set; }

        public string StreetName { get; set; }

    }
}