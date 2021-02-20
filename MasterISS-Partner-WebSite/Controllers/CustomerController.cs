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
        public ActionResult NewCustomer()
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
                ViewBag.ProvincesByGeneralInfo = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByCorporativeResidency = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByCorporativeCompany = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesByIndividual = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                ViewBag.ProvincesBySetup = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
            }
            ViewBag.DistrictsByGeneralInfo = new SelectList("");
            ViewBag.RuralRegionsByGeneralInfo = new SelectList("");
            ViewBag.NeigboorHoodsByGeneralInfo = new SelectList("");
            ViewBag.StreetsByGeneralInfo = new SelectList("");
            ViewBag.BuildingsByGeneralInfo = new SelectList("");
            ViewBag.ApartmentsByGeneralInfo = new SelectList("");
            ViewBag.DistrictsBySetup = new SelectList("");
            ViewBag.RuralRegionsBySetup = new SelectList("");
            ViewBag.NeigboorHoodsBySetup = new SelectList("");
            ViewBag.StreetsBySetup = new SelectList("");
            ViewBag.BuildingsBySetup = new SelectList("");
            ViewBag.ApartmentsBySetup = new SelectList("");
            ViewBag.DistrictsByIndividual = new SelectList("");
            ViewBag.RuralRegionsByIndividual =
            ViewBag.NeigboorHoodsByIndividual = new SelectList("");
            ViewBag.StreetsByIndividual = new SelectList("");
            ViewBag.BuildingsByIndividual = new SelectList("");
            ViewBag.ApartmentsByIndividual = new SelectList("");
            ViewBag.DistrictsByCorporativeResidency = new SelectList("");
            ViewBag.RuralRegionsByCorporativeResidency = new SelectList("");
            ViewBag.NeigboorHoodsByCorporativeResidency = new SelectList("");
            ViewBag.StreetsByCorporativeResidency = new SelectList("");
            ViewBag.BuildingsByCorporativeResidency = new SelectList("");
            ViewBag.ApartmentsByCorporativeResidency = new SelectList("");
            ViewBag.DistrictsByCorporativeCompany = new SelectList("");
            ViewBag.RuralRegionsByCorporativeCompany = new SelectList("");
            ViewBag.NeigboorHoodsByCorporativeCompany = new SelectList("");
            ViewBag.StreetsByCorporativeCompany = new SelectList("");
            ViewBag.BuildingsByCorporativeCompany = new SelectList("");
            ViewBag.ApartmentsByCorporativeCompany = new SelectList("");
            ViewBag.BillingPeriod = new SelectList("");

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
                        RemoveModel("Individual.ResidencyAddress");
                    }
                }
                else
                {
                    RemoveModel("Individual");
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeCompanyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.CompanyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                        RemoveModel("CorporateInfo.CompanyAddress");
                    }
                    if (addCustomerViewModel.CorporateInfo.SameSetupAddressByCorporativeResidencyAddress == true)
                    {
                        ChangeAddressInfo(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                        RemoveModel("CorporateInfo.ExecutiveResidencyAddress");
                    }
                }

                if (addCustomerViewModel.GeneralInfo.SameSetupAddressByBilling == true)
                {
                    ChangeAddressInfo(addCustomerViewModel.GeneralInfo.BillingAddress, addCustomerViewModel.SubscriptionInfo.SetupAddress);
                    RemoveModel("GeneralInfo.BillingAddress");
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


                    if (setupAddress.ResponseMessage.ErrorCode == 0 && billingAddress.ResponseMessage.ErrorCode == 0 && (addCustomerViewModel.Individual.ResidencyAddress.ApartmentId != null || addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId != null))
                    {
                        addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(billingAddress.AddressDetailsResponse);

                        addCustomerViewModel.SubscriptionInfo.SetupAddress.NewCustomerAddressInfoRequest = NewCustomerAddressInfoRequest(setupAddress.AddressDetailsResponse);


                        var wrapperBySMSConfirmation = new WebServiceWrapper();

                        var smsConfirmation = wrapperBySMSConfirmation.SendConfirmationSMS(addCustomerViewModel.GeneralInfo.ContactPhoneNo);

                        if (smsConfirmation.ResponseMessage.ErrorCode == 0)
                        {
                            Session["CustomerApplicationInfo"] = addCustomerViewModel;
                            Session["SMSCode"] = smsConfirmation.SMSCodeResponse.Code;
                            return View("SmsConfirmation");
                        }
                        else
                        {
                            ViewBag.NewCustomerError = "SMS ile ilgili bir sıkıntı oldu.";
                        }
                    }
                    else
                    {
                        ViewBag.NewCustomerError = Localization.View.NewCustomerError;
                    }
                }
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

            ViewBag.BillingPeriod = PaymentDayListByViewBag(addCustomerViewModel.SubscriptionInfo.PartnerTariffID ?? null, addCustomerViewModel.SubscriptionInfo.BillingPeriodId ?? null);

            //GeneralInfo=>Billing
            ViewBag.ProvincesByGeneralInfo = ProvincesList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null);
            ViewBag.DistrictsByGeneralInfo = DistrictList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByGeneralInfo = RuralRegionsList(addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByGeneralInfo = NeighborhoodList(addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByGeneralInfo = StreetList(addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null);
            ViewBag.BuildingsByGeneralInfo = BuildingList(addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null);
            ViewBag.ApartmentsByGeneralInfo = ApartmentList(addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId ?? null);

            //SubscriptionInfo=>Setup
            ViewBag.ProvincesBySetup = ProvincesList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null);
            ViewBag.DistrictsBySetup = DistrictList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null);
            ViewBag.RuralRegionsBySetup = RuralRegionsList(addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsBySetup = NeighborhoodList(addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null);
            ViewBag.StreetsBySetup = StreetList(addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null);
            ViewBag.BuildingsBySetup = BuildingList(addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null);
            ViewBag.ApartmentsBySetup = ApartmentList(addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId ?? null);

            if (addCustomerViewModel.GeneralInfo.CustomerTypeId == (int)CustomerTypeEnum.Individual)
            {
                ViewBag.ProvincesByIndividual = ProvincesList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null);
                ViewBag.DistrictsByIndividual = DistrictList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null, addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null);
                ViewBag.RuralRegionsByIndividual = RuralRegionsList(addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null, addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null);
                ViewBag.NeigboorHoodsByIndividual = NeighborhoodList(addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null);
                ViewBag.StreetsByIndividual = StreetList(addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null);
                ViewBag.BuildingsByIndividual = BuildingList(addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null, addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null);
                ViewBag.ApartmentsByIndividual = ApartmentList(addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null, addCustomerViewModel.Individual.ResidencyAddress.ApartmentId ?? null);

                ViewBag.ProvincesByCorporativeResidency = new SelectList("");
                ViewBag.DistrictsByCorporativeResidency = new SelectList("");
                ViewBag.RuralRegionsByCorporativeResidency = new SelectList("");
                ViewBag.NeigboorHoodsByCorporativeResidency = new SelectList("");
                ViewBag.StreetsByCorporativeResidency = new SelectList("");
                ViewBag.BuildingsByCorporativeResidency = new SelectList("");
                ViewBag.ApartmentsByCorporativeResidency = new SelectList("");
                ViewBag.ProvincesByCorporativeCompany = new SelectList("");
                ViewBag.DistrictsByCorporativeCompany = new SelectList("");
                ViewBag.RuralRegionsByCorporativeCompany = new SelectList("");
                ViewBag.NeigboorHoodsByCorporativeCompany = new SelectList("");
                ViewBag.StreetsByCorporativeCompany = new SelectList("");
                ViewBag.BuildingsByCorporativeCompany = new SelectList("");
                ViewBag.ApartmentsByCorporativeCompany = new SelectList("");
            }

            else//This is Corporative
            {
                ViewBag.ProvincesByCorporativeResidency = ProvincesList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null);
                ViewBag.DistrictsByCorporativeResidency = DistrictList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null);
                ViewBag.RuralRegionsByCorporativeResidency = RuralRegionsList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null);
                ViewBag.NeigboorHoodsByCorporativeResidency = NeighborhoodList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null);
                ViewBag.StreetsByCorporativeResidency = StreetList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null);
                ViewBag.BuildingsByCorporativeResidency = BuildingList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null);
                ViewBag.ApartmentsByCorporativeResidency = ApartmentList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId ?? null);


                ViewBag.ProvincesByCorporativeCompany = ProvincesList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null);
                ViewBag.DistrictsByCorporativeCompany = DistrictList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null);
                ViewBag.RuralRegionsByCorporativeCompany = RuralRegionsList(addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null);
                ViewBag.NeigboorHoodsByCorporativeCompany = NeighborhoodList(addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null);
                ViewBag.StreetsByCorporativeCompany = StreetList(addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null);
                ViewBag.BuildingsByCorporativeCompany = BuildingList(addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null);
                ViewBag.ApartmentsByCorporativeCompany = ApartmentList(addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId ?? null);

                ViewBag.ProvincesByIndividual = new SelectList("");
                ViewBag.DistrictsByIndividual = new SelectList("");
                ViewBag.RuralRegionsByIndividual = new SelectList("");
                ViewBag.NeigboorHoodsByIndividual = new SelectList("");
                ViewBag.StreetsByIndividual = new SelectList("");
                ViewBag.BuildingsByIndividual = new SelectList("");
                ViewBag.ApartmentsByIndividual = new SelectList("");
            }


            return View(addCustomerViewModel);

        }


        public ActionResult Successful()
        {
            return View();
        }


        [HttpPost]
        public ActionResult PaymentDayList(long? id)
        {
            var wrapper = new WebServiceWrapper();

            var paymentDayListResponse = wrapper.GetPaymentDays((long)id);

            if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
            {
                var list = paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }).ToArray();

                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorMessage = paymentDayListResponse.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SmsConfirmation(string inputCode)
        {
            var serviceCode = Session["SMSCode"] == null ? null : Session["SMSCode"].ToString();
            if (!string.IsNullOrEmpty(serviceCode))
            {
                if (Convert.ToInt32(Session["Counter"]) < 3)
                {
                    if (inputCode == serviceCode)
                    {
                        var wrapper = new WebServiceWrapper();

                        var customerApplicationInfo = Session["CustomerApplicationInfo"] as AddCustomerViewModel;

                        var response = wrapper.NewCustomerRegister(customerApplicationInfo);

                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            Session.Remove("CustomerApplicationInfo");
                            Session.Remove("Counter");
                            Session.Remove("SMSCode");

                            return RedirectToAction("Successful");
                        }
                        else
                        {
                            return View("NewCustomer", customerApplicationInfo);
                        }
                    }
                    else
                    {
                        ViewBag.Error = "yanlış girdin";
                        Session["Counter"] = Convert.ToInt32(Session["Counter"]) + 1;
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("NewCustomer");
                }
            }

            var customerAppInfo = Session["CustomerApplicationInfo"] as AddCustomerViewModel;
            return View("NewCustomer", customerAppInfo);
        }


        private SelectList PaymentDayListByViewBag(long? tariffId, long? selectedValue)
        {
            if (tariffId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var paymentDayListResponse = wrapper.GetPaymentDays((long)tariffId);

                if (paymentDayListResponse.ResponseMessage.ErrorCode == 0)
                {
                    var list = new SelectList(paymentDayListResponse.KeyValueItemResponse.Select(tck => new { Name = tck.Value, Value = tck.Key }), "Value", "Name", selectedValue);
                    return list;
                }
            }
            return new SelectList("");
        }

        private SelectList ProvincesList(long? selectedValue)
        {
            var wrapper = new WebServiceWrapper();
            var provinceList = wrapper.GetProvince();

            if (!string.IsNullOrEmpty(provinceList.ErrorMessage))
            {
                //var nullList = new SelectList("");
                //return nullList;

                //Hata Olursa Nolucak????????????????
            }
            var list = new SelectList(provinceList.Data.ValueNamePairList.Select(tck => new { Name = tck.Name, Value = tck.Value }), "Value", "Name", selectedValue);

            return list;
        }

        private SelectList DistrictList(long? provinceId, long? selectedValue)
        {
            if (provinceId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var districtList = wrapper.GetDistricts((long)provinceId);
                var list = new SelectList(districtList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");

        }

        private SelectList RuralRegionsList(long? districtId, long? selectedValue)
        {
            if (districtId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var ruralRegionsList = wrapper.GetRuralRegions((long)districtId);
                var list = new SelectList(ruralRegionsList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        private SelectList NeighborhoodList(long? ruralRegionsId, long? selectedValue)
        {
            if (ruralRegionsId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var neighborhoodList = wrapper.GetNeigbourhood((long)ruralRegionsId);
                var list = new SelectList(neighborhoodList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        private SelectList StreetList(long? neighborhoodId, long? selectedValue)
        {
            if (neighborhoodId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var streetList = wrapper.GetStreets((long)neighborhoodId);
                var list = new SelectList(streetList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        private SelectList BuildingList(long? streetId, long? selectedValue)
        {
            if (streetId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var buildList = wrapper.GetBuildings((long)streetId);
                var list = new SelectList(buildList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name", selectedValue);
                return list;
            }
            return new SelectList("");
        }

        private SelectList ApartmentList(long? buildingId, long? selectedValue)
        {
            if (buildingId.HasValue)
            {
                var wrapper = new WebServiceWrapper();
                var apartmentList = wrapper.GetApartments((long)buildingId);
                var list = new SelectList(apartmentList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }), "Value", "Name");
                return list;
            }
            return new SelectList("");

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
            changingViewModel.BuildingId = changerViewModel.BuildingId;
            changingViewModel.DistrictId = changerViewModel.DistrictId;
            changingViewModel.NeighborhoodId = changerViewModel.NeighborhoodId;
            changingViewModel.ProvinceId = changerViewModel.ProvinceId;
            changingViewModel.RuralRegionsId = changerViewModel.RuralRegionsId;
            changingViewModel.StreetId = changerViewModel.StreetId;
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