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
        public long? ProvinceId { get; set; }

        public long? DistrictId { get; set; }

        public long? RuralRegionsId { get; set; }

        public long? NeighborhoodId { get; set; }

        public long? StreetId { get; set; }

        public long? BuildingId { get; set; }

        [Required]
        public long? ApartmentId { get; set; }

        [Required]
        public int? PostalCode { get; set; }

        [Required]
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