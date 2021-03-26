using MasterISS_Partner_WebSite_WebServices.CustomerSetupServiceReference;
using MasterISS_Partner_WebSite.ViewModels.Setup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class SetupServiceWrapper
    {
        private readonly string Username;

        private readonly string Rand;

        private readonly string Culture;

        private readonly string KeyFragment;

        private readonly string Password;
        private CustomerSetupServiceClient Client { get; set; }

        public SetupServiceWrapper()
        {
            var claimInfo = new ClaimInfo();
            Username = claimInfo.PartnerSetupServiceUser();
            Culture = CultureInfo.CurrentCulture.ToString();
            KeyFragment = new CustomerSetupServiceClient().GetKeyFragment(claimInfo.PartnerSetupServiceUser());
            Rand = Guid.NewGuid().ToString("N");
            Password = claimInfo.PartnerSetupServiceHash();
            Client = new CustomerSetupServiceClient();
        }

        private string SetupHash()
        {
            var wrapper = new WebServiceWrapper();
            var hashAuthenticaiton = wrapper.CalculateHash<SHA256>(Username + Rand + Password + KeyFragment);
            return hashAuthenticaiton;
        }

        public AddCustomerAttachmentResponse AddCustomerAttachment(UploadFileRequestViewModel uploadFileViewModel)
        {
            var request = new AddCustomerAttachmentRequest
            {
                Culture = Culture,
                Hash = SetupHash(),
                Rand = Rand,
                Username = Username,
                CustomerAttachment = new CustomerAttachment
                {
                    FileData = uploadFileViewModel.FileData,
                    FileType = uploadFileViewModel.Extension.Replace(".", ""),
                    TaskNo = (long)uploadFileViewModel.TaskNo,
                    AttachmentType = (short)uploadFileViewModel.AttachmentTypesEnum
                }
            };

            var response = Client.AddCustomerAttachment(request);

            return response;
        }

        //public ParameterlessResponse AddTaskStatusUpdate(AddTaskStatusUpdateViewModel taskStatusUpdateViewModel)
        //{
        //    var request = new AddTaskStatusUpdateRequest
        //    {
        //        Culture = Culture,
        //        Hash = SetupHash(),
        //        Rand = Rand,
        //        Username = Username,
        //        TaskUpdate = new TaskUpdate
        //        {
        //            TaskNo = (long)taskStatusUpdateViewModel.TaskNo,
        //            Description = taskStatusUpdateViewModel.Description,
        //            FaultCode = (short)taskStatusUpdateViewModel.FaultCodes,
        //            ReservationDate = DateTimeConvertedBySetupWebService(taskStatusUpdateViewModel.ReservationDate)
        //        },
        //    };
        //    var response = Client.AddTaskStatusUpdate(request);

        //    return response;
        //}

        public GetCustomerAttachmentsResponse GetCustomerAttachments(long taskNo)
        {
            var response = Client.GetCustomerAttachments(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerContractResponse GetCustomerContract(long taskNo)
        {
            var response = Client.GetCustomerContract(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerCredentialResponse GetCustomerCredentials(long taskNo)
        {
            var response = Client.GetCustomerCredentials(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerLineDetailsResponse GetCustomerLineDetails(long taskNo)
        {
            var response = Client.GetCustomerLineDetails(TaskNoRequest(taskNo));

            return response;
        }

        public GetCustomerSessionInfoResponse GetCustomerSessionInfo(long taskNo)
        {
            var response = Client.GetCustomerSessionInfo(TaskNoRequest(taskNo));

            return response;
        }

        public GetTaskDetailsResponse GetTaskDetails(long taskNo)
        {
            var response = Client.GetTaskDetails(TaskNoRequest(taskNo));

            return response;
        }

        //public GetTaskListResponse GetTaskList(GetTaskListRequestViewModel taskListRequestViewModel)
        //{
        //    var request = new GetTaskListRequest
        //    {
        //        Culture = Culture,
        //        Hash = SetupHash(),
        //        Rand = Rand,
        //        Username = Username,
        //        DateSpan = new DateSpan
        //        {
        //            EndDate = DateTimeConvertedBySetupWebService(taskListRequestViewModel.TaskListEndDate),
        //            StartDate = DateTimeConvertedBySetupWebService(taskListRequestViewModel.TaskListStartDate),
        //        }
        //    };

        //    var response = Client.GetTaskList(request);

        //    return response;
        //}

        public ParameterlessResponse UpdateClientLocation(UpdateClientGPSRequestViewModel updateClientGPSRequestViewModel)
        {
            var longitude = Convert.ToDecimal(updateClientGPSRequestViewModel.Longitude, new CultureInfo("en-US"));
            var latitude = Convert.ToDecimal(updateClientGPSRequestViewModel.Latitude, new CultureInfo("en-US"));

            var request = new UpdateCustomerLocationRequest
            {
                Culture = Culture,
                Hash = SetupHash(),
                Rand = Rand,
                Username = Username,
                LocationUpdate = new LocationUpdate
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    TaskNo = updateClientGPSRequestViewModel.TaskNo
                }
            };
            var response = Client.UpdateClientLocation(request);

            return response;
        }



        private TaskNoRequest TaskNoRequest(long taskNo)
        {
            var request = new TaskNoRequest
            {
                Culture = Culture,
                Hash = SetupHash(),
                Rand = Rand,
                Username = Username,
                TaskNo = taskNo
            };

            return request;
        }
        private string DateTimeConvertedBySetupWebService(string dateToFormatted)
        {
            if (!string.IsNullOrEmpty(dateToFormatted))
            {
                var formattedDate = DateTime.ParseExact(dateToFormatted, "dd.MM.yyyy HH:mm", null).ToString("yyyy-MM-dd HH:mm:ss");
                return formattedDate;
            }
            return null;
        }

    }
}