﻿using MasterISS_Partner_WebSite_WebServices.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite.ViewModels.Home;
using NLog;
using RadiusR.DB.Enums;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Partner_WebSite_Enums;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize(Roles = "Sale")]
    [Authorize(Roles = "Admin,SaleManager")]
    public class CustomerController : BaseController
    {
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");



        // GET: Customer
        public ActionResult NewCustomer()
        {
            var wrapper = new WebServiceWrapper();

            ViewBag.SubscriptionRegistrationType = SubscriptionRegistrationType(null);

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
                ViewBag.NationalityListByCorporative = NationalityList(nationalityListResponse, (int)MasterISS_Partner_WebSite_Enums.NationalityList.Turkey);
                ViewBag.NationalityListByIndividual = NationalityList(nationalityListResponse, (int)MasterISS_Partner_WebSite_Enums.NationalityList.Turkey);
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
            ViewBag.RuralRegionsByIndividual = new SelectList("");
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

            ViewBag.DayList = DayList(null);
            ViewBag.MonthList = Monthlist(null);
            ViewBag.YearList = YearList(null);

            ViewBag.DayListByExpiryDate = DayList(null);
            ViewBag.MonthListByExpiryDate = Monthlist(null);
            ViewBag.YearListByExpiryDate = YearList(null);

            ViewBag.DayListByIssueDate = DayList(null);
            ViewBag.MonthListByIssueDate = Monthlist(null);
            ViewBag.YearListByIssueDate = YearList(null);


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(AddCustomerViewModel addCustomerViewModel)
        {
            if (addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId.HasValue && addCustomerViewModel.IDCard.CardTypeId.HasValue && addCustomerViewModel.GeneralInfo.CustomerTypeId.HasValue && !string.IsNullOrEmpty(addCustomerViewModel.SubscriptionInfo.SetupAddress.Floor) && addCustomerViewModel.SubscriptionInfo.SetupAddress.PostalCode.HasValue && addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId.HasValue)
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

                if (Enum.IsDefined(typeof(RadiusR.DB.Enums.SubscriptionRegistrationType), addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId))
                {
                    if (addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId != (int)RadiusR.DB.Enums.SubscriptionRegistrationType.Transition)
                    {
                        addCustomerViewModel.ExtraInfo.XDSLNo = null;
                        ModelState.Remove("ExtraInfo.XDSLNo");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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
                        else
                        {
                            serviceWrapper = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while GetApartmentAddress , individualAddress: {individualAddress.ResponseMessage.ErrorCode} , by: {serviceWrapper.GetUserSubMail()}");

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
                        else
                        {
                            webServiceWrapper = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while GetApartmentAddress , CompanyApartmentAddressResponse: {companyApartmentAddress.ResponseMessage.ErrorCode} ExecutiveResidencyBBKResponseCode: {executiveResidencyAddress.ResponseMessage.ErrorCode}, by: {webServiceWrapper.GetUserSubMail()}");
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
                            //LOG
                            wrapperBySMSConfirmation = new WebServiceWrapper();
                            LoggerError.Fatal("An error occurred while SMSConfirmation , ErrorCode: " + smsConfirmation.ResponseMessage.ErrorCode + ", by: " + wrapperBySMSConfirmation.GetUserSubMail());
                            //LOG

                            ViewBag.NewCustomerError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(smsConfirmation.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);

                        }
                    }
                    else
                    {
                        //LOG
                        wrapperByQueryApartmentAddress = new WebServiceWrapper();
                        LoggerError.Fatal($"An error occurred while GetApartmentAddress , ErrorCode: BillingAddressResponse : {billingAddress.ResponseMessage.ErrorCode} SetupAddressResponse : {setupAddress.ResponseMessage.ErrorCode}, by: {wrapperByQueryApartmentAddress.GetUserSubMail()}");
                        //LOG

                        ViewBag.NewCustomerError = Localization.View.NewCustomerError;
                    }
                }
            }

            ViewBag.SubscriptionRegistrationType = SubscriptionRegistrationType(addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId ?? null);

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

            var addressInfo = new AddressInfo();
            //GeneralInfo=>Billing
            ViewBag.ProvincesByGeneralInfo = addressInfo.ProvincesList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null);
            ViewBag.DistrictsByGeneralInfo = addressInfo.DistrictList(addCustomerViewModel.GeneralInfo.BillingAddress.ProvinceId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByGeneralInfo = addressInfo.RuralRegionsList(addCustomerViewModel.GeneralInfo.BillingAddress.DistrictId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByGeneralInfo = addressInfo.NeighborhoodList(addCustomerViewModel.GeneralInfo.BillingAddress.RuralRegionsId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByGeneralInfo = addressInfo.StreetList(addCustomerViewModel.GeneralInfo.BillingAddress.NeighborhoodId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null);
            ViewBag.BuildingsByGeneralInfo = addressInfo.BuildingList(addCustomerViewModel.GeneralInfo.BillingAddress.StreetId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null);
            ViewBag.ApartmentsByGeneralInfo = addressInfo.ApartmentList(addCustomerViewModel.GeneralInfo.BillingAddress.BuildingId ?? null, addCustomerViewModel.GeneralInfo.BillingAddress.ApartmentId ?? null);

            //SubscriptionInfo=>Setup
            ViewBag.ProvincesBySetup = addressInfo.ProvincesList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null);
            ViewBag.DistrictsBySetup = addressInfo.DistrictList(addCustomerViewModel.SubscriptionInfo.SetupAddress.ProvinceId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null);
            ViewBag.RuralRegionsBySetup = addressInfo.RuralRegionsList(addCustomerViewModel.SubscriptionInfo.SetupAddress.DistrictId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsBySetup = addressInfo.NeighborhoodList(addCustomerViewModel.SubscriptionInfo.SetupAddress.RuralRegionsId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null);
            ViewBag.StreetsBySetup = addressInfo.StreetList(addCustomerViewModel.SubscriptionInfo.SetupAddress.NeighborhoodId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null);
            ViewBag.BuildingsBySetup = addressInfo.BuildingList(addCustomerViewModel.SubscriptionInfo.SetupAddress.StreetId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null);
            ViewBag.ApartmentsBySetup = addressInfo.ApartmentList(addCustomerViewModel.SubscriptionInfo.SetupAddress.BuildingId ?? null, addCustomerViewModel.SubscriptionInfo.SetupAddress.ApartmentId ?? null);

            //Individual=>ResidencyAddress
            ViewBag.ProvincesByIndividual = addressInfo.ProvincesList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByIndividual = addressInfo.DistrictList(addCustomerViewModel.Individual.ResidencyAddress.ProvinceId ?? null, addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByIndividual = addressInfo.RuralRegionsList(addCustomerViewModel.Individual.ResidencyAddress.DistrictId ?? null, addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByIndividual = addressInfo.NeighborhoodList(addCustomerViewModel.Individual.ResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByIndividual = addressInfo.StreetList(addCustomerViewModel.Individual.ResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null);
            ViewBag.BuildingsByIndividual = addressInfo.BuildingList(addCustomerViewModel.Individual.ResidencyAddress.StreetId ?? null, addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByIndividual = addressInfo.ApartmentList(addCustomerViewModel.Individual.ResidencyAddress.BuildingId ?? null, addCustomerViewModel.Individual.ResidencyAddress.ApartmentId ?? null);

            //CorporateInfo=>ExecutiveResidencyAddress
            ViewBag.ProvincesByCorporativeResidency = addressInfo.ProvincesList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByCorporativeResidency = addressInfo.DistrictList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByCorporativeResidency = addressInfo.RuralRegionsList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByCorporativeResidency = addressInfo.NeighborhoodList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByCorporativeResidency = addressInfo.StreetList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null);
            ViewBag.BuildingsByCorporativeResidency = addressInfo.BuildingList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByCorporativeResidency = addressInfo.ApartmentList(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.ApartmentId ?? null);

            //CorporateInfo=>CompanyAddress
            ViewBag.ProvincesByCorporativeCompany = addressInfo.ProvincesList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null);
            ViewBag.DistrictsByCorporativeCompany = addressInfo.DistrictList(addCustomerViewModel.CorporateInfo.CompanyAddress.ProvinceId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null);
            ViewBag.RuralRegionsByCorporativeCompany = addressInfo.RuralRegionsList(addCustomerViewModel.CorporateInfo.CompanyAddress.DistrictId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null);
            ViewBag.NeigboorHoodsByCorporativeCompany = addressInfo.NeighborhoodList(addCustomerViewModel.CorporateInfo.CompanyAddress.RuralRegionsId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null);
            ViewBag.StreetsByCorporativeCompany = addressInfo.StreetList(addCustomerViewModel.CorporateInfo.CompanyAddress.NeighborhoodId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null);
            ViewBag.BuildingsByCorporativeCompany = addressInfo.BuildingList(addCustomerViewModel.CorporateInfo.CompanyAddress.StreetId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null);
            ViewBag.ApartmentsByCorporativeCompany = addressInfo.ApartmentList(addCustomerViewModel.CorporateInfo.CompanyAddress.BuildingId ?? null, addCustomerViewModel.CorporateInfo.CompanyAddress.ApartmentId ?? null);

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
            return Json(new { errorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(paymentDayListResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SmsConfirmation(string inputCode)
        {
            var serviceCode = Session["SMSCode"]?.ToString();
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

                            //LOG
                            wrapper = new WebServiceWrapper();
                            Logger.Info("Added Customer: " + customerApplicationInfo.IDCard.FirstName + customerApplicationInfo.IDCard.LastName + ", by: " + wrapper.GetUserSubMail());
                            //LOG

                            return RedirectToAction("Successful");
                        }
                        else
                        {
                            Session.Remove("CustomerApplicationInfo");
                            Session.Remove("Counter");
                            Session.Remove("SMSCode");
                            //LOG
                            wrapper = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while NewCustomerRegister, ErrorCode:  {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}, NameValuePair :{string.Join(",", response.NewCustomerRegisterResponse)}  by: {wrapper.GetUserSubMail()}");
                            //LOG

                            TempData["SMSConfirmationError"] = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                            return RedirectToAction("NewCustomer");
                        }
                    }
                    else
                    {
                        ViewBag.Error = Localization.View.WrongPassword;
                        Session["Counter"] = Convert.ToInt32(Session["Counter"]) + 1;

                        return View();
                    }
                }
                else
                {
                    TempData["SMSConfirmationError"] = Localization.View.SMSCode3TimesIncorrectlyError;
                    Session.Remove("Counter");
                    Session.Remove("CustomerApplicationInfo");
                    Session.Remove("SMSCode");
                    return RedirectToAction("NewCustomer");
                }
            }
            Session.Remove("CustomerApplicationInfo");
            TempData["SMSConfirmationError"] = Localization.View.GeneralErrorDescription;
            return RedirectToAction("NewCustomer");
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
                IDCard.TCIDCardWithChip = null;
                RemoveModel("IDCard.TCIDCardWithChip");
            }
            else
            {
                IDCard.TCBirthCertificate = null;
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

        private SelectList SubscriptionRegistrationType(int? selectedValue)
        {
            var registrationTypeLocalized = new LocalizedList<RadiusR.DB.Enums.SubscriptionRegistrationType, RadiusR.Localization.Lists.SubscriptionRegistrationType>().GetList(CultureInfo.CurrentCulture).Where(s => s.Key != (int)RadiusR.DB.Enums.SubscriptionRegistrationType.Transfer);
            var list = new SelectList(registrationTypeLocalized.Select(r => new { Value = r.Key, Name = r.Value }), "Value", "Name", selectedValue);
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

        private SelectList YearList(int? selectedValue)
        {
            var startDate = 1940;
            var yearList = new List<int>();

            for (int i = startDate; i < DateTime.Now.Year; i++)
            {
                yearList.Add(i);
            }

            var list = new SelectList(yearList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }

        private SelectList Monthlist(int? selectedValue)
        {
            var monthList = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                monthList.Add(i);
            }

            var list = new SelectList(monthList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }

        private SelectList DayList(int? selectedValue)
        {
            var dayList = new List<int>();

            for (int i = 1; i <= 31; i++)
            {
                dayList.Add(i);
            }

            var list = new SelectList(dayList.Select(yl => new { Name = yl.ToString(), Value = yl }), "Value", "Name", selectedValue ?? null);
            return list;
        }
    }
}