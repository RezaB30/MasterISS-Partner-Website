using MasterISS_Partner_WebSite.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            var wrapper = new WebServiceWrapper();

            var tckTypeResponse = wrapper.GetTCKTypes();
            if (tckTypeResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.TckTypeList = TCKTypeList(tckTypeResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var sexListResponse = wrapper.GetSexesList();
            if (sexListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.SexexList = SexesList(sexListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var cultureListResponse = wrapper.GetCultures();
            if (cultureListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.CultureList = CultureList(cultureListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var professionsListResponse = wrapper.GetProfessions();
            if (professionsListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.ProfessionList = ProfessionList(professionsListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var nationalityListResponse = wrapper.GetNationalities();
            if (nationalityListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.NationalityList = NationalityList(nationalityListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var partnerTariffListResponse = wrapper.GetPartnerTariffs();
            if (partnerTariffListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.PartnerTariffList = PartnerTariffList(partnerTariffListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var customerTypeList = wrapper.GetCustomerType();
            if (customerTypeList.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.CustomerTypeList = CustomerTypeList(customerTypeList, null);
            }


            wrapper = new WebServiceWrapper();
            var provinceList = wrapper.GetProvince();

            if (string.IsNullOrEmpty(provinceList.ErrorMessage))
            {
                ViewBag.Provinces = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(AddCustomerViewModel addCustomerViewModel)
        {
            var companyBBK = addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId;
            var executiveResidencyBBK = addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId;
            var billingAddressBBK = addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId;

            var wrapper = new WebServiceWrapper();
            var companyApartmentAddress = wrapper.GetApartmentAddress((long)companyBBK);

            wrapper = new WebServiceWrapper();
            var executiveResidencyAddress = wrapper.GetApartmentAddress((long)executiveResidencyBBK);

            wrapper = new WebServiceWrapper();
            var billingAddress = wrapper.GetApartmentAddress((long)billingAddressBBK);

            if (billingAddress.ResponseMessage.ErrorCode == 0 && companyApartmentAddress.ResponseMessage.ErrorCode == 0 && executiveResidencyAddress.ResponseMessage.ErrorCode == 0)
            {
                addCustomerViewModel.CorporateInfo.CompanyAddress.NewCustomerAddressInfoRequest = new NewCustomerAddressInfoRequest()
                {
                    AddressText = companyApartmentAddress.AddressDetailsResponse.AddressText,
                    AddressNo = companyApartmentAddress.AddressDetailsResponse.AddressNo,
                    AparmentNo = companyApartmentAddress.AddressDetailsResponse.ApartmentNo,
                    ApartmentId = companyApartmentAddress.AddressDetailsResponse.ApartmentID,
                    DistrictId = companyApartmentAddress.AddressDetailsResponse.DistrictID,
                    DistrictName = companyApartmentAddress.AddressDetailsResponse.DistrictName,
                    DoorId = companyApartmentAddress.AddressDetailsResponse.DoorID,
                    DoorNo = companyApartmentAddress.AddressDetailsResponse.DoorNo,
                    NeighbourhoodID = companyApartmentAddress.AddressDetailsResponse.NeighbourhoodID,
                    NeighbourhoodName = companyApartmentAddress.AddressDetailsResponse.NeighbourhoodName,
                    ProvinceId = companyApartmentAddress.AddressDetailsResponse.ProvinceID,
                    ProvinceName = companyApartmentAddress.AddressDetailsResponse.ProvinceName,
                    RuralCode = companyApartmentAddress.AddressDetailsResponse.RuralCode,
                    StreetId = companyApartmentAddress.AddressDetailsResponse.StreetID,
                    StreetName = companyApartmentAddress.AddressDetailsResponse.StreetName,
                };

                addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NewCustomerAddressInfoRequest = new NewCustomerAddressInfoRequest()
                {
                    AddressText = executiveResidencyAddress.AddressDetailsResponse.AddressText,
                    AddressNo = executiveResidencyAddress.AddressDetailsResponse.AddressNo,
                    AparmentNo = executiveResidencyAddress.AddressDetailsResponse.ApartmentNo,
                    ApartmentId = executiveResidencyAddress.AddressDetailsResponse.ApartmentID,
                    DistrictId = executiveResidencyAddress.AddressDetailsResponse.DistrictID,
                    DistrictName = executiveResidencyAddress.AddressDetailsResponse.DistrictName,
                    DoorId = executiveResidencyAddress.AddressDetailsResponse.DoorID,
                    DoorNo = executiveResidencyAddress.AddressDetailsResponse.DoorNo,
                    NeighbourhoodID = executiveResidencyAddress.AddressDetailsResponse.NeighbourhoodID,
                    NeighbourhoodName = executiveResidencyAddress.AddressDetailsResponse.NeighbourhoodName,
                    ProvinceId = executiveResidencyAddress.AddressDetailsResponse.ProvinceID,
                    ProvinceName = executiveResidencyAddress.AddressDetailsResponse.ProvinceName,
                    RuralCode = executiveResidencyAddress.AddressDetailsResponse.RuralCode,
                    StreetId = executiveResidencyAddress.AddressDetailsResponse.StreetID,
                    StreetName = executiveResidencyAddress.AddressDetailsResponse.StreetName,
                };

                addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest = new NewCustomerAddressInfoRequest()
                {
                    AddressText = billingAddress.AddressDetailsResponse.AddressText,
                    AddressNo = billingAddress.AddressDetailsResponse.AddressNo,
                    AparmentNo = billingAddress.AddressDetailsResponse.ApartmentNo,
                    ApartmentId = billingAddress.AddressDetailsResponse.ApartmentID,
                    DistrictId = billingAddress.AddressDetailsResponse.DistrictID,
                    DistrictName = billingAddress.AddressDetailsResponse.DistrictName,
                    DoorId = billingAddress.AddressDetailsResponse.DoorID,
                    DoorNo = billingAddress.AddressDetailsResponse.DoorNo,
                    NeighbourhoodID = billingAddress.AddressDetailsResponse.NeighbourhoodID,
                    NeighbourhoodName = billingAddress.AddressDetailsResponse.NeighbourhoodName,
                    ProvinceId = billingAddress.AddressDetailsResponse.ProvinceID,
                    ProvinceName = billingAddress.AddressDetailsResponse.ProvinceName,
                    RuralCode = billingAddress.AddressDetailsResponse.RuralCode,
                    StreetId = billingAddress.AddressDetailsResponse.StreetID,
                    StreetName = billingAddress.AddressDetailsResponse.StreetName,
                };
            }

            //Burada ındexteki viewbagları tekrar taşıman gerekicek
            ViewBag.Error = "Adresler gelmiyor, sıkıntı";//bu mesajıda değiştir
            return View("Index");
        }

        [HttpPost]
        public ActionResult PaymentDayList(long id)
        {
            var wrapper = new WebServiceWrapper();

            var paymentDayListResponse = wrapper.GetPaymentDays(id);

            if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
            {
                var list = paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }).ToArray();

                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorMessage = paymentDayListResponse.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }


        private SelectList CustomerTypeList(PartnerServiceKeyValueListResponse customerType, int? selectedValue)
        {
            var list = new SelectList(customerType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList PartnerTariffList(PartnerServiceKeyValueListResponse partnerTarifType, int? selectedValue)
        {
            var list = new SelectList(partnerTarifType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList NationalityList(PartnerServiceKeyValueListResponse nationalityType, int? selectedValue)
        {
            var list = new SelectList(nationalityType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList ProfessionList(PartnerServiceKeyValueListResponse professionType, int? selectedValue)
        {
            var list = new SelectList(professionType.KeyValueItemResponse.Select(tck => new { Name = tck.Value.ToUpper(), Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList CultureList(PartnerServiceKeyValueListResponse cultureType, string selectedValue)
        {
            var list = new SelectList(cultureType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Value }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList SexesList(PartnerServiceKeyValueListResponse sexesType, int? selectedValue)
        {
            var list = new SelectList(sexesType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private SelectList TCKTypeList(PartnerServiceKeyValueListResponse tckType, int? selectedValue)
        {
            var list = new SelectList(tckType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }
    }
}