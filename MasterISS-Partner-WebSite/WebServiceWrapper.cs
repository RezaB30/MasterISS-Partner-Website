using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
            Username = MasterISS_Partner_WebSite.Properties.Settings.Default.Username;
            Culture = CultureInfo.CurrentCulture.ToString();
            KeyFragment = new PartnerServiceClient().GetKeyFragment(MasterISS_Partner_WebSite.Properties.Settings.Default.Username);
            Rand = Guid.NewGuid().ToString("N");
            Password = MasterISS_Partner_WebSite.Properties.Settings.Default.Password;
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

        public ServiceResponse<CustomerServiceAddressDetailsResponse> GetApartmentAddress(long BBK)
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

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<CustomerServiceAddressDetailsResponse>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<CustomerServiceAddressDetailsResponse>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
        }

        public ServiceResponse<PartnerServiceBillListResponse> UserBillList(string subscriberNo)
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

            if (response.ResponseMessage.ErrorCode == 0)
            {
                return new ServiceResponse<PartnerServiceBillListResponse>()
                {
                    Data = response
                };
            }
            return new ServiceResponse<PartnerServiceBillListResponse>()
            {
                ErrorMessage = response.ResponseMessage.ErrorMessage
            };
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
                    UserEmail = userSignInModel.DealerCode,
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
                    RequestedSubUserPassword = newUserViewModel.Password
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
        public string PartenrSetupServiceUser()
        {
            var partnerSetupServiceUser = CurrentClaims().Where(c => c.Type == "SetupServiceUser")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerSetupServiceUser;
        }
        public string PartnerSetupServiceHash()
        {
            var partnerSetupServiceHash = CurrentClaims().Where(c => c.Type == "SetupServiceHash")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerSetupServiceHash;
        }
        public string PartnerId()
        {
            var partnerId = CurrentClaims().Where(c => c.Type == "UserId")
                  .Select(c => c.Value).SingleOrDefault();
            return partnerId;
        }
        public string GetPartnerName()
        {
            var partnerName = CurrentClaims().Where(c => c.Type == "PartnerName")
                   .Select(c => c.Value).SingleOrDefault();
            return partnerName;
        }
        public string Hash<HAT>() where HAT : HashAlgorithm
        {
            var hashAuthenticaiton = CalculateHash<HAT>(Username + Rand + CalculateHash<HAT>(Password) + KeyFragment);
            return hashAuthenticaiton;
        }
        public string GetUserPassword()
        {
            var userPassword = CurrentClaims().Where(c => c.Type == "UserPassword")
                 .Select(c => c.Value).SingleOrDefault();
            return userPassword;
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
        private List<Claim> CurrentClaims()
        {
            return ClaimsPrincipal.Current.Identities.First().Claims.ToList();


            //var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
            //                   .Select(c => c.Value).SingleOrDefault();
        }
        private string GetUserMail()
        {
            var userMail = CurrentClaims().Where(c => c.Type == "UserMail")
                   .Select(c => c.Value).SingleOrDefault();
            return userMail;
        }
        private string GetUserSubMail()
        {
            var userSubMail = CurrentClaims().Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();
            return userSubMail;
        }
        public string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
    }
}