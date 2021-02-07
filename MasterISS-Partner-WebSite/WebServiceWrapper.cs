using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public string Hash()
        {
            var hashAuthenticaiton = CalculateHash<SHA256>(Username + Rand + CalculateHash<SHA256>(Password) + KeyFragment);
            return hashAuthenticaiton;
        }

        private string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }

        public ServiceResponse<PartnerServicePaymentResponse> PayBill(PayBillRequestViewModel requestModel)
        {
            var request = new PartnerServicePaymentRequest()
            {
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username,
                PaymentRequest = new PaymentRequest()
                {
                    SubUserEmail = requestModel.SubMail,
                    BillIDs = requestModel.BillIDs,
                }
            };
            var response = Client.PayBills(request);

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
                Hash = Hash(),
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
                Hash = Hash(),
                Rand = Rand,
                Username = Username
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

        public ServiceResponse<CustomerServiceAddressDetailsResponse> GetOpenAdress(long BBK)
        {
            var request = new CustomerServiceAddressDetailsRequest
            {
                BBK = BBK,
                Culture = Culture,
                Hash = Hash(),
                Rand = Rand,
                Username = Username
            };

            var response = Client.GetApartmentAddress(request);

            var aresponse = Client.AddSubUser(new PartnerServiceAddSubUserRequest() { AddSubUserRequestParameters= new AddSubUserRequest() { } });

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

        private CustomerServiceNameValuePairRequest GetRequest(long id)
        {
            var request = new CustomerServiceNameValuePairRequest
            {
                Culture = Culture,
                Hash = Hash(),
                ItemCode = id,
                Rand = Rand,
                Username = Username
            };
            return request;
        }

    }
}