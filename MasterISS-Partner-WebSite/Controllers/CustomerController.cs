using MasterISS_Partner_WebSite.Enums;
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
                ViewBag.SexexListByCorporative = SexesList(sexListResponse, null);
                ViewBag.SexexListByIndividual = SexesList(sexListResponse, null);
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
                ViewBag.ProfessionListByCorporative = ProfessionList(professionsListResponse, null);
                ViewBag.ProfessionListByIndividual = ProfessionList(professionsListResponse, null);
            }

            wrapper = new WebServiceWrapper();

            var nationalityListResponse = wrapper.GetNationalities();
            if (nationalityListResponse.ResponseMessage.ErrorCode == 0)
            {
                ViewBag.NationalityListByCorporative = NationalityList(nationalityListResponse, null);
                ViewBag.NationalityListByIndividual = NationalityList(nationalityListResponse, null);
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
            if (addCustomerViewModel.IDCard.CardTypeId.HasValue && addCustomerViewModel.GeneralInfo.CustomerTypeId.HasValue && !string.IsNullOrEmpty(addCustomerViewModel.SubscriptionInfo.SetupAddress.Floor) && addCustomerViewModel.SubscriptionInfo.SetupAddress.PostalCode.HasValue && addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId.HasValue)
            {
                if (IsCustomerTypeIndividual((int)addCustomerViewModel.GeneralInfo.CustomerTypeId))
                {
                    RemoveModel("CorporateInfo");
                    if (addCustomerViewModel.Individual.SameSetupAddressByIndividual == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.Individual.ResidencyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);

                        ModelState.Remove("Individual.ResidencyAddress.ApartmentId");
                        ModelState.Remove("Individual.ResidencyAddress.Floor");
                        ModelState.Remove("Individual.ResidencyAddress.PostalCode");
                    }
                }
                else
                {
                    RemoveModel("Individual");
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeCompanyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.CompanyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);

                        ModelState.Remove("CorporateInfo.CompanyAddress.ApartmentId");
                        ModelState.Remove("CorporateInfo.CompanyAddress.PostalCode");
                        ModelState.Remove("CorporateInfo.CompanyAddress.Floor");
                    }
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeResidencyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);

                        ModelState.Remove("CorporateInfo.ExecutiveResidencyAddress.ApartmentId");
                        ModelState.Remove("CorporateInfo.ExecutiveResidencyAddress.Floor");
                        ModelState.Remove("CorporateInfo.ExecutiveResidencyAddress.PostalCode");
                    }
                }

                if (addCustomerViewModel.GeneralInfo.SameSetupAddressByBilling == true)
                {
                    ChangeAddressInfo(addCustomerViewModel.GeneralInfo.BillingAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);

                    ModelState.Remove("GeneralInfo.BillingAddress.ApartmentId");
                    ModelState.Remove("GeneralInfo.BillingAddress.PostalCode");
                    ModelState.Remove("GeneralInfo.BillingAddress.Floor");
                }

                IdCardValidationAndRemoveModelState((int)addCustomerViewModel.IDCard.CardTypeId, addCustomerViewModel.IDCard);

                if (ModelState.IsValid)
                {
                    if (IsCustomerTypeIndividual((int)addCustomerViewModel.GeneralInfo.CustomerTypeId))
                    {
                        var individualResidencyBBK = addCustomerViewModel.Individual.ResidencyAddress.ApartmentId;

                        var serviceWrapper = new WebServiceWrapper();
                        var individualAddress = serviceWrapper.GetApartmentAddress((long)individualResidencyBBK);

                        if (individualAddress.ResponseMessage.ErrorCode == 0)
                        {
                            addCustomerViewModel.Individual.ResidencyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(individualAddress.AddressDetailsResponse);
                        }
                    }
                    else//This Is Corporative
                    {
                        var companyBBK = addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId;
                        var executiveResidencyBBK = addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId;

                        var webServiceWrapper = new WebServiceWrapper();
                        var companyApartmentAddress = webServiceWrapper.GetApartmentAddress((long)companyBBK);

                        webServiceWrapper = new WebServiceWrapper();
                        var executiveResidencyAddress = webServiceWrapper.GetApartmentAddress((long)executiveResidencyBBK);

                        if (companyApartmentAddress.ResponseMessage.ErrorCode == 0 && executiveResidencyAddress.ResponseMessage.ErrorCode == 0)
                        {
                            addCustomerViewModel.CorporateInfo.CompanyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(companyApartmentAddress.AddressDetailsResponse);

                            addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(executiveResidencyAddress.AddressDetailsResponse);
                        }
                    }

                    var billingAddressBBK = addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId;
                    var setupAddressBBK = addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId;

                    var wrapperByQueryApartmentAddress = new WebServiceWrapper();
                    var billingAddress = wrapperByQueryApartmentAddress.GetApartmentAddress((long)billingAddressBBK);

                    wrapperByQueryApartmentAddress = new WebServiceWrapper();
                    var setupAddress = wrapperByQueryApartmentAddress.GetApartmentAddress((long)setupAddressBBK);


                    if (setupAddress.ResponseMessage.ErrorCode == 0 && billingAddress.ResponseMessage.ErrorCode == 0)
                    {
                        addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(billingAddress.AddressDetailsResponse);

                        addCustomerViewModel.SubscriptionInfo.SetupAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(setupAddress.AddressDetailsResponse);

                        var wrapperByNewCustomer = new WebServiceWrapper();
                        var response = wrapperByNewCustomer.NewCustomerRegister(addCustomerViewModel);

                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            return RedirectToAction("Successful");//Bu sayfayı ekle
                        }
                        else
                        {
                            ViewBag.Error = "kayıt eklerken bir hata oldu, sıkıntı";
                        }
                    }
                    else
                    {
                        //Web Servisler çalışmazsa
                        ViewBag.Error = "Adresler gelmiyor, sıkıntı";//bu mesajı değiştir
                    }
                }
                else
                {
                    //Modelstate hatası
                    ViewBag.Error = "düzgün doldur bilgileri";//bu mesajıda değiştir
                }

                var wrapper = new WebServiceWrapper();

                var tckTypeResponse = wrapper.GetTCKTypes();
                if (tckTypeResponse.ResponseMessage.ErrorCode == 0)
                {
                    ViewBag.TckTypeList = TCKTypeList(tckTypeResponse, addCustomerViewModel.IDCard.CardTypeId ?? null);
                }

                wrapper = new WebServiceWrapper();

                var sexListResponse = wrapper.GetSexesList();
                if (sexListResponse.ResponseMessage.ErrorCode == 0)
                {
                    ViewBag.SexexListByCorporative = SexesList(sexListResponse, addCustomerViewModel.CorporateInfo.ExecutiveSexId ?? null);
                    ViewBag.SexexListByIndividual = SexesList(sexListResponse, addCustomerViewModel.Individual.SexId ?? null);
                }

                wrapper = new WebServiceWrapper();

                var cultureListResponse = wrapper.GetCultures();
                if (cultureListResponse.ResponseMessage.ErrorCode == 0)
                {
                    ViewBag.CultureList = CultureList(cultureListResponse, addCustomerViewModel.GeneralInfo.Culture);
                }

                wrapper = new WebServiceWrapper();

                var professionsListResponse = wrapper.GetProfessions();
                if (professionsListResponse.ResponseMessage.ErrorCode == 0)
                {
                    ViewBag.ProfessionListByCorporative = ProfessionList(professionsListResponse, addCustomerViewModel.CorporateInfo.ExecutiveProfessionId ?? null);
                    ViewBag.ProfessionListByIndividual = ProfessionList(professionsListResponse, addCustomerViewModel.Individual.ProfessionId ?? null);
                }

                wrapper = new WebServiceWrapper();

                var nationalityListResponse = wrapper.GetNationalities();
                if (nationalityListResponse.ResponseMessage.ErrorCode == 0)
                {
                    ViewBag.NationalityListByCorporative = NationalityList(nationalityListResponse, addCustomerViewModel.CorporateInfo.ExecutiveNationalityId ?? null);
                    ViewBag.NationalityListByIndividual = NationalityList(nationalityListResponse, addCustomerViewModel.Individual.ProfessionId ?? null);
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
                    ViewBag.CustomerTypeList = CustomerTypeList(customerTypeList, addCustomerViewModel.GeneralInfo.CustomerTypeId ?? null);
                }

                wrapper = new WebServiceWrapper();
                var provinceList = wrapper.GetProvince();

                if (string.IsNullOrEmpty(provinceList.ErrorMessage))
                {
                    ViewBag.Provinces = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                }

                return View("Index", addCustomerViewModel);
            }
            else
            {
                TempData["CustomerIdValid"] = "Customer Type Id boş bırakma";
                TempData["SetupApartmentIdValid"] = "Setup Apartment Id Id boş bırakma";
                return RedirectToAction("Index");
            }
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

        private bool IsCustomerTypeIndividual(int customerTypeId)
        {
            if (customerTypeId == (int)CustomerTypeEnum.Individual)
            {
                return true;
            }
            return false;
        }

        private void ChangeAddressInfo(AddressInfoViewModel changingViewModel, AddressInfoViewModel changerViewModel)
        {
            changingViewModel.ApartmentId = changerViewModel.ApartmentId;
            changingViewModel.PostalCode = changerViewModel.PostalCode;
            changingViewModel.Floor = changerViewModel.Floor;
        }

        private void IdCardValidationAndRemoveModelState(int cardTypeId, IDCardViewModel IDCard)
        {
            if (cardTypeId == (int)CardTypeEnum.TCBirthCertificate)
            {
                RemoveModel("IDCard.TCIDCardWithChip");
            }
            else
            {
                RemoveModel("IDCard.TCBirthCertificate");
            }
        }

        private void RemoveModel(string removedModel)
        {
            var removedModelKeys = ModelState.Keys.Where(k => k.Contains(removedModel)).ToArray();
            foreach (var key in removedModelKeys)
            {
                ModelState.Remove(key);
            }
        }

        private SelectList CustomerTypeList(PartnerServiceKeyValueListResponse customerType, int? selectedValue)
        {
            var list = new SelectList(customerType.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
            return list;
        }

        private NewCustomerAddressInfoRequest NewCustomerAddressInfoRequest(AddressDetailsResponse addressDetailsResponse)
        {
            var request = new NewCustomerAddressInfoRequest
            {
                AddressText = addressDetailsResponse.AddressText,
                AddressNo = addressDetailsResponse.AddressNo,
                ApartmentNo = addressDetailsResponse.ApartmentNo,
                ApartmentId = addressDetailsResponse.ApartmentID,
                DistrictId = addressDetailsResponse.DistrictID,
                DistrictName = addressDetailsResponse.DistrictName,
                DoorId = addressDetailsResponse.DoorID,
                DoorNo = addressDetailsResponse.DoorNo,
                NeighbourhoodID = addressDetailsResponse.NeighbourhoodID,
                NeighbourhoodName = addressDetailsResponse.NeighbourhoodName,
                ProvinceId = addressDetailsResponse.ProvinceID,
                ProvinceName = addressDetailsResponse.ProvinceName,
                RuralCode = addressDetailsResponse.RuralCode,
                StreetId = addressDetailsResponse.StreetID,
                StreetName = addressDetailsResponse.StreetName,
            };
            return request;
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