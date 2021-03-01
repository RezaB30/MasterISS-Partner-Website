using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite_WebServices.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels.Home;
using MasterISS_Partner_WebSite.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class WebServiceWrapper
    {
        private readonly string Username;

        private readonly string Rand;

        private readonly string Culture;

        private readonly string KeyFragment;

        private readonly string Password;
        private PartnerServiceClient Client { get; set; }

        public WebServiceWrapper()
        {
            Username = Properties.Settings.Default.Username;
            Culture = CultureInfo.CurrentCulture.ToString();
            KeyFragment = new PartnerServiceClient().GetKeyFragment(Properties.Settings.Default.Username);
            Rand = Guid.NewGuid().ToString("N");
            Password = Properties.Settings.Default.Password;
            Client = new PartnerServiceClient();
        }

        public ServiceResponse<PartnerServicePaymentResponse> PayBill(long[] BillIds)
        {
            var request = new PartnerServicePaymentRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PaymentRequest = new PaymentRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    BillIDs = BillIds,
                }
            };
            var client = new PartnerServiceClient();
            var response = client.PayBills(request);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<PartnerServicePaymentResponse>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<PartnerServicePaymentResponse>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceServiceAvailabilityResponse> ServiceAvailability(string BBK)
        {
            var request = new CustomerServiceServiceAvailabilityRequest
            {
                Culture = Culture,
                Hash = Hash<SHA1>(),
                Rand = Rand,
                Username = Username,
                ServiceAvailabilityParameters = new ServiceAvailabilityRequest()
                {
                    bbk = BBK,
                }
            };
            var response = Client.ServiceAvailability(request);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceServiceAvailabilityResponse>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceServiceAvailabilityResponse>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetProvince()
        {

            var request = new CustomerServiceProvincesRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA1>(),
                Rand = Rand,
                Username = Username,
            };
            var response = Client.GetProvinces(request);

            if (response.ResponseMessage.ErrorCode == 0)//Success
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetDistricts(long id)
        {
            var response = Client.GetProvinceDistricts(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetRuralRegions(long id)
        {
            var response = Client.GetDistrictRuralRegions(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetNeigbourhood(long id)
        {
            var response = Client.GetRuralRegionNeighbourhoods(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetStreets(long id)
        {
            var response = Client.GetNeighbourhoodStreets(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetBuildings(long id)
        {
            var response = Client.GetStreetBuildings(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<CustomerServiceNameValuePair> GetApartments(long id)
        {
            var response = Client.GetBuildingApartments(GetRequest(id));

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceNameValuePair>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceNameValuePair>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public CustomerServiceAddressDetailsResponse GetApartmentAddress(long BBK)
        {
            var request = new CustomerServiceAddressDetailsRequest
            {
                BBK = BBK,
                Culture = Culture,
                Hash = Hash<SHA1>(),
                Rand = Rand,
                Username = Username
            };

            var response = Client.GetApartmentAddress(request);

            return response;
        }

        public PartnerServiceBillListResponse UserBillList(string subscriberNo)
        {
            var request = new PartnerServiceBillListRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                BillListRequest = new BillListRequest()
                {
                    SubscriberNo = subscriberNo,
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail()
                }
            };
            var response = Client.BillsBySubscriberNo(request);

            return response;
        }

        public PartnerServiceAuthenticationResponse Authenticate(UserSignInViewModel userSignInModel)
        {
            var request = new PartnerServiceAuthenticationRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                AuthenticationParameters = new AuthenticationRequest()
                {
                    SubUserEmail = userSignInModel.Username,
                    UserEmail = userSignInModel.PartnerCode,
                    PartnerPasswordHash = CalculateHash<SHA256>(userSignInModel.Password),
                }
            };

            var response = Client.Authenticate(request);

            return response;
        }

        public PartnerServiceAddSubUserResponse AddUser(NewUserViewModel newUserViewModel)
        {
            var request = new PartnerServiceAddSubUserRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                AddSubUserRequestParameters = new AddSubUserRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    RequestedSubUserEmail = newUserViewModel.UserEmail,
                    RequestedSubUserName = newUserViewModel.UserNameSurname,
                    RequestedSubUserPassword = CalculateHash<SHA256>(newUserViewModel.Password)
                }
            };

            var response = Client.AddSubUser(request);

            return response;
        }

        public PartnerServiceSubUserResponse EnableUser(string enabledUserMail)
        {
            var response = Client.EnableSubUser(PartnerServiceSubUserRequest(enabledUserMail));

            return response;
        }

        public PartnerServiceSubUserResponse DisableUser(string disabledUserMail)
        {
            var response = Client.DisableSubUser(PartnerServiceSubUserRequest(disabledUserMail));
            return response;
        }

        public PartnerServiceKeyValueListResponse GetSexesList()
        {
            var response = Client.GetSexes(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceKeyValueListResponse GetCultures()
        {
            var response = Client.GetCultures(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceCreditReportResponse GetCreditReportWithDetail()
        {
            var response = Client.GetCreditReport(PartnerServiceCreditReportRequest(true));
            return response;
        }

        public PartnerServiceCreditReportResponse GetCreditReportNotDetail()
        {
            var response = Client.GetCreditReport(PartnerServiceCreditReportRequest(false));
            return response;
        }

        public PartnerServiceKeyValueListResponse GetCustomerType()
        {
            var response = Client.GetCustomerTypes(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceKeyValueListResponse GetNationalities()
        {
            var response = Client.GetNationalities(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceKeyValueListResponse GetPartnerTariffs()
        {
            var response = Client.GetPartnerTariffs(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceKeyValueListResponse GetPaymentDays(long Id)
        {
            var request = new PartnerServiceListFromIDRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                ListFromIDRequest = new ListFromIDRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    ID = Id,
                }
            };

            var response = Client.GetPaymentDays(request);

            return response;
        }

        public PartnerServiceKeyValueListResponse GetProfessions()
        {
            var response = Client.GetProfessions(PartnerServiceParameterlessRequest());
            return response;
        }

        public PartnerServiceKeyValueListResponse GetTCKTypes()
        {
            var response = Client.GetTCKTypes(PartnerServiceParameterlessRequest());

            return response;
        }

        public PartnerServiceIDCardValidationResponse IDCardValidation(IDCardValidationViewModel IDCardValidationViewModel)
        {
            var request = new PartnerServiceIDCardValidationRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                IDCardValidationRequest = new IDCardValidationRequest
                {
                    BirthDate = IDCardValidationViewModel.BirtDate,
                    RegistirationNo = IDCardValidationViewModel.RegistirationNo,
                    FirstName = IDCardValidationViewModel.FirstName,
                    IDCardType = IDCardValidationViewModel.IdCardType,
                    LastName = IDCardValidationViewModel.LastName,
                    TCKNo = IDCardValidationViewModel.TCKNo,
                }
            };

            var response = Client.IDCardValidation(request);

            return response;
        }

        public PartnerServiceNewCustomerRegisterResponse NewCustomerRegister(AddCustomerViewModel addCustomerViewModel)
        {
            var parseDatetime = new DatetimeParse();
            var request = new PartnerServiceNewCustomerRegisterRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                CustomerRegisterParameters = new NewCustomerRegisterRequest
                {
                    UserEmail = GetUserMail(),
                    SubUserEmail = GetUserSubMail(),

                    CorporateCustomerInfo = addCustomerViewModel.GeneralInfo.CustomerTypeId == (int)Enums.CustomerTypeEnum.Individual ? null : new CorporateCustomerInfo
                    {
                        CentralSystemNo = addCustomerViewModel.CorporateInfo.CentralSystemNo,
                        ExecutiveBirthPlace = addCustomerViewModel.CorporateInfo.ExecutiveBirthPlace,
                        ExecutiveFathersName = addCustomerViewModel.CorporateInfo.ExecutiveFathersName,
                        ExecutiveMothersMaidenName = addCustomerViewModel.CorporateInfo.ExecutiveMothersMaidenName,
                        ExecutiveMothersName = addCustomerViewModel.CorporateInfo.ExecutiveMothersName,
                        ExecutiveNationality = addCustomerViewModel.CorporateInfo.ExecutiveNationalityId,
                        ExecutiveProfession = addCustomerViewModel.CorporateInfo.ExecutiveProfessionId,
                        ExecutiveSex = addCustomerViewModel.CorporateInfo.ExecutiveSexId,
                        TaxNo = addCustomerViewModel.CorporateInfo.TaxNo,
                        TaxOffice = addCustomerViewModel.CorporateInfo.TaxOffice,
                        Title = addCustomerViewModel.CorporateInfo.Title,
                        TradeRegistrationNo = addCustomerViewModel.CorporateInfo.TradeRegistrationNo,
                        ExecutiveResidencyAddress = NewAddressInfo(addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.PostalCode, addCustomerViewModel.CorporateInfo.ExecutiveResidencyAddress.Floor),
                        CompanyAddress = NewAddressInfo(addCustomerViewModel.CorporateInfo.CompanyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.CorporateInfo.CompanyAddress.PostalCode, addCustomerViewModel.CorporateInfo.CompanyAddress.Floor)
                    },

                    CustomerGeneralInfo = new CustomerGeneralInfo()
                    {
                        BillingAddress = NewAddressInfo(addCustomerViewModel.GeneralInfo.BillingAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.GeneralInfo.BillingAddress.PostalCode, addCustomerViewModel.GeneralInfo.BillingAddress.Floor),
                        ContactPhoneNo = addCustomerViewModel.GeneralInfo.ContactPhoneNo,
                        Email = addCustomerViewModel.GeneralInfo.Email,
                        CustomerType = addCustomerViewModel.GeneralInfo.CustomerTypeId,
                        Culture = addCustomerViewModel.GeneralInfo.Culture,
                        OtherPhoneNos = addCustomerViewModel.GeneralInfo.OtherPhoneNos.Select(phoneNo => new PhoneNoListItem()
                        {
                            Number = phoneNo
                        }).ToArray()
                    },

                    IDCardInfo = new IDCardInfo()
                    {
                        BirthDate = parseDatetime.ConvertDatetimeByWebService(addCustomerViewModel.IDCard.BirthDate),
                        CardType = addCustomerViewModel.IDCard.CardTypeId,
                        DateOfIssue = DateOfIssueValue((int)addCustomerViewModel.IDCard.CardTypeId, addCustomerViewModel.IDCard),
                        District = addCustomerViewModel.IDCard.TCBirthCertificate.District,
                        FirstName = addCustomerViewModel.IDCard.FirstName,
                        LastName = addCustomerViewModel.IDCard.LastName,
                        Neighbourhood = addCustomerViewModel.IDCard.TCBirthCertificate.Neighbourhood,
                        PageNo = addCustomerViewModel.IDCard.TCBirthCertificate.PageNo,
                        PassportNo = addCustomerViewModel.IDCard.PassportNo,
                        PlaceOfIssue = addCustomerViewModel.IDCard.TCBirthCertificate.PlaceOfIssue,
                        Province = addCustomerViewModel.IDCard.TCBirthCertificate.Province,
                        RowNo = addCustomerViewModel.IDCard.TCBirthCertificate.RowNo,
                        SerialNo = addCustomerViewModel.IDCard.SerialNo,
                        TCKNo = addCustomerViewModel.IDCard.TCKNo,
                        VolumeNo = addCustomerViewModel.IDCard.TCBirthCertificate.VolumeNo,
                    },

                    IndividualCustomerInfo = addCustomerViewModel.GeneralInfo.CustomerTypeId != (int)Enums.CustomerTypeEnum.Individual ? null : new IndividualCustomerInfo()
                    {
                        BirthPlace = addCustomerViewModel.Individual.BirthPlace,
                        FathersName = addCustomerViewModel.Individual.FathersName,
                        MothersMaidenName = addCustomerViewModel.Individual.MothersMaidenName,
                        MothersName = addCustomerViewModel.Individual.MothersName,
                        Nationality = addCustomerViewModel.Individual.NationalityId,
                        Profession = addCustomerViewModel.Individual.ProfessionId,
                        Sex = addCustomerViewModel.Individual.SexId,
                        ResidencyAddress = NewAddressInfo(addCustomerViewModel.Individual.ResidencyAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.Individual.ResidencyAddress.PostalCode, addCustomerViewModel.Individual.ResidencyAddress.Floor)
                    },

                    SubscriptionInfo = new SubscriptionRegistrationInfo()
                    {
                        BillingPeriod = addCustomerViewModel.SubscriptionInfo.BillingPeriodId,
                        ServiceID = addCustomerViewModel.SubscriptionInfo.PartnerTariffID,
                        SetupAddress = NewAddressInfo(addCustomerViewModel.SubscriptionInfo.SetupAddress.NewCustomerAddressInfoRequest, (int)addCustomerViewModel.SubscriptionInfo.SetupAddress.PostalCode, addCustomerViewModel.SubscriptionInfo.SetupAddress.Floor)
                    },
                    ExtraInfo = new ExtraInfo()
                    {
                        ApplicationType = addCustomerViewModel.ExtraInfo.SubscriptionRegistrationTypeId,
                        PSTN = addCustomerViewModel.ExtraInfo.PSTN,
                        XDSLNo = addCustomerViewModel.ExtraInfo.XDSLNo
                    }
                },
            };

            var response = Client.NewCustomerRegister(request);

            return response;
        }

        public PartnerServiceAllowanceDetailsResponse GetBasicAllowanceDetails(GetBasicAllowDetailViewModel getBasicAllowDetailViewModel)
        {
            var claimInfo = new ClaimInfo();

            var request = new PartnerServiceBasicAllowanceRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PartnerBasicAllowanceRequest = new PartnerBasicAllowanceRequest()
                {
                    AllowanceTypeId = (short?)getBasicAllowDetailViewModel.RevenuesTypeEnum,
                    ItemPerPage = getBasicAllowDetailViewModel.PageInfo.ItemPerPage,
                    PageNo = getBasicAllowDetailViewModel.PageInfo.PageNo,
                    PartnerId = claimInfo.PartnerId()
                }
            };

            var response = Client.GetBasicAllowanceDetails(request);

            return response;
        }

        public PartnerServiceSetupGenericAllowanceListResponse SetupGenericAllowanceList(PageSettingsByWebService pageSettings)
        {

            var response = Client.SetupGenericAllowanceList(PartnerServiceAllowanceRequest(pageSettings));

            return response;
        }

        public PartnerServiceSetupGenericAllowanceListResponse SetupAllowanceDetails(SetupAllowanceDetailsRequest setupAllowanceDetailsRequest)
        {
            
            var response = Client.SetupAllowanceDetails(PartnerServiceAllowanceDetailRequest(setupAllowanceDetailsRequest));

            return response;
        }

        public PartnerServiceSetupAllowanceListResponse SetupAllowanceList(PageSettingsByWebService pageSettings)
        {
            
            var response = Client.SetupAllowanceList(PartnerServiceAllowanceRequest(pageSettings));

            return response;
        }

        public PartnerServiceSaleGenericAllowanceListResponse SaleGenericAllowanceList(PageSettingsByWebService pageSettings)
        {
            var response = Client.SaleGenericAllowanceList(PartnerServiceAllowanceRequest(pageSettings));

            return response;
        }

        public PartnerServiceSaleGenericAllowanceListResponse SaleAllowanceDetails(SetupAllowanceDetailsRequest setupAllowanceDetailsRequest)
        {
            var response = Client.SaleAllowanceDetails(PartnerServiceAllowanceDetailRequest(setupAllowanceDetailsRequest));

            return response;
        }

        public PartnerServiceSaleAllowanceListResponse SaleAllowanceList(PageSettingsByWebService pageSettings)
        {
            var response = Client.SaleAllowanceList(PartnerServiceAllowanceRequest(pageSettings));

            return response;
        }



        private PartnerServiceAllowanceDetailRequest PartnerServiceAllowanceDetailRequest(SetupAllowanceDetailsRequest setupAllowanceDetailsRequest)
        {
            var claimInfo = new ClaimInfo();
            var request = new PartnerServiceAllowanceDetailRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PartnerAllowanceDetailRequest = new PartnerAllowanceDetailRequest()
                {
                    PartnerId = claimInfo.PartnerId(),
                    PageNo = setupAllowanceDetailsRequest.PageInfo.PageNo,
                    ItemPerPage = setupAllowanceDetailsRequest.PageInfo.ItemPerPage,
                    AllowanceCollectionID = setupAllowanceDetailsRequest.AllowanceCollectionID,
                },
            };
            return request;
        }

        private PartnerServiceAllowanceRequest PartnerServiceAllowanceRequest(PageSettingsByWebService pageSettings)
        {
            var claimInfo = new ClaimInfo();
            var request = new PartnerServiceAllowanceRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                PartnerAllowanceRequest = new PartnerAllowanceRequest()
                {
                    ItemPerPage = pageSettings.ItemPerPage,
                    PageNo = pageSettings.PageNo,
                    PartnerId = claimInfo.PartnerId(),
                },
            };
            return request;
        }

        private string DateOfIssueValue(int cardTypeId, IDCardViewModel IDCardViewModel)
        {
            var parseDatetime = new DatetimeParse();

            if (cardTypeId == (int)Enums.CardTypeEnum.TCBirthCertificate)
            {
                var dateOfIssue = parseDatetime.ConvertDatetimeByWebService(IDCardViewModel.TCBirthCertificate.DateOfIssue);
                return dateOfIssue;
            }
            var expiryDate = parseDatetime.ConvertDatetimeByWebService(IDCardViewModel.TCIDCardWithChip.ExpiryDate);
            return expiryDate;
        }

        public PartnerServiceSMSCodeResponse SendConfirmationSMS(string phoneNo)
        {
            var request = new PartnerServiceSMSCodeRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                SMSCodeRequest = new SMSCodeRequest
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    PhoneNo = phoneNo,
                },
            };
            var response = Client.SendConfirmationSMS(request);

            return response;
        }
        public string Hash<HAT>() where HAT : HashAlgorithm
        {
            var hashAuthenticaiton = CalculateHash<HAT>(Username + Rand + CalculateHash<HAT>(Password) + KeyFragment);
            return hashAuthenticaiton;
        }
        private AddressInfo NewAddressInfo(NewCustomerAddressInfoRequest addressInfoRequest, int postalCode, string floor)
        {
            var request = new AddressInfo()
            {
                AddressNo = addressInfoRequest.AddressNo,
                AddressText = addressInfoRequest.AddressText,
                ApartmentID = addressInfoRequest.ApartmentId,
                ApartmentNo = addressInfoRequest.ApartmentNo,
                DistrictID = addressInfoRequest.DistrictId,
                DistrictName = addressInfoRequest.DistrictName,
                DoorID = addressInfoRequest.DoorId,
                DoorNo = addressInfoRequest.DoorNo,
                NeighbourhoodID = addressInfoRequest.NeighbourhoodID,
                NeighbourhoodName = addressInfoRequest.NeighbourhoodName,
                ProvinceID = addressInfoRequest.ProvinceId,
                ProvinceName = addressInfoRequest.ProvinceName,
                RuralCode = addressInfoRequest.RuralCode,
                StreetID = addressInfoRequest.StreetId,
                StreetName = addressInfoRequest.StreetName,
                PostalCode = postalCode,
                Floor = floor
            };
            return request;
        }
        private CustomerServiceNameValuePairRequest GetRequest(long id)
        {
            var request = new CustomerServiceNameValuePairRequest
            {
                Culture = Culture,
                Hash = Hash<SHA1>(),
                ItemCode = id,
                Rand = Rand,
                Username = Username
            };
            return request;
        }
        private PartnerServiceParameterlessRequest PartnerServiceParameterlessRequest()
        {
            var request = new PartnerServiceParameterlessRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                ParameterlessRequest = new ParameterlessRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                }
            };
            return request;
        }
        private PartnerServiceCreditReportRequest PartnerServiceCreditReportRequest(bool isDetail)
        {
            var request = new PartnerServiceCreditReportRequest()
            {
                Username = Username,
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                CreditReportRequest = new CreditReportRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    WithDetails = isDetail
                }
            };
            return request;
        }
        private PartnerServiceSubUserRequest PartnerServiceSubUserRequest(string subUserMail)
        {
            var request = new PartnerServiceSubUserRequest()
            {
                Culture = Culture,
                Hash = Hash<SHA256>(),
                Rand = Rand,
                Username = Username,
                SubUserRequest = new SubUserRequest()
                {
                    SubUserEmail = GetUserSubMail(),
                    UserEmail = GetUserMail(),
                    RequestedSubUserEmail = subUserMail,
                }
            };
            return request;
        }
        public string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
        private string GetUserMail()
        {
            var claimInfo = new ClaimInfo();

            var userMail = claimInfo.CurrentClaims().Where(c => c.Type == "UserMail")
                   .Select(c => c.Value).SingleOrDefault();
            return userMail;
        }
        public string GetUserSubMail()
        {
            var claimInfo = new ClaimInfo();

            var userSubMail = claimInfo.CurrentClaims().Where(c => c.Type == ClaimTypes.Email)
                   .Select(c => c.Value).SingleOrDefault();
            return userSubMail;
        }

    }
}