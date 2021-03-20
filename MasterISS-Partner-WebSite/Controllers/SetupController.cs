using MasterISS_Partner_WebSite.ViewModels.Setup;
using MasterISS_Partner_WebSite.ViewModels.Setup.Response;
using PagedList;
using System;
using System.Globalization;
using System.Linq;
using RezaB.Data;
using System.Web.Mvc;
using RezaB.Data.Localization;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using GoogleMaps.LocationServices;
using System.Net;
using System.Xml.Linq;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using System.Collections.Generic;
using MasterISS_Partner_WebSite.ViewModels;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize(Roles = "Setup")]
    [Authorize(Roles = "Admin,SetupManager")]
    public class SetupController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private TimeSpan firtSessionTime;
        private TimeSpan lastSessionTime;

        // GET: Setup
        public ActionResult Index([Bind(Prefix = "search")] GetTaskListRequestViewModel taskListRequestModel, int page = 1, int pageSize = 20)
        {
            taskListRequestModel = taskListRequestModel ?? new GetTaskListRequestViewModel();

            if (taskListRequestModel.TaskListStartDate == null && taskListRequestModel.TaskListEndDate == null)
            {
                taskListRequestModel.TaskListStartDate = DateTime.Now.AddDays(-29);
                taskListRequestModel.TaskListEndDate = DateTime.Now;
            }

            else if (taskListRequestModel.TaskListStartDate != null && taskListRequestModel.TaskListEndDate == null)
            {
                //var regex = new Regex(@"^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d (20|21|22|23|[0-1]?\d):[0-5]?\d+");//dd.MM.yyyy HH:mm"

                //var isMatch = regex.Match(taskListRequestModel.TaskListStartDate.ToString());

                //if (isMatch.Success)
                //{

                var startDateConverted = taskListRequestModel.TaskListStartDate;

                taskListRequestModel.TaskListEndDate = startDateConverted.Value.AddDays(29);

                //}
                //else
                //{
                //    ViewBag.ErrorMessage = Localization.View.StartDateValid;
                //}
            }

            if (ModelState.IsValid)
            {
                var startDate = taskListRequestModel.TaskListStartDate;
                var endDate = taskListRequestModel.TaskListEndDate;

                if (startDate <= endDate)
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
                        if (startDate.Value.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                        {
                            var taskList = TaskList(User.IsInRole("Admin"));

                            //var ass = taskList.First().TaskIssueDate;

                            var list = taskList.Where(tlresponse => tlresponse.TaskIssueDate >= startDate && tlresponse.TaskIssueDate <= endDate).Where(tlresponse => taskListRequestModel.SearchedTaskNo.HasValue == true ? tlresponse.TaskNo == taskListRequestModel.SearchedTaskNo : tlresponse.ContactName.Contains(taskListRequestModel.SearchedName == null ? "" : taskListRequestModel.SearchedName.ToUpper())).Select(tl => new GetTaskListResponseViewModel
                            {
                                AddressLatitudeandLongitude = GetAddressLatituteandLongitude(tl.Address),
                                ContactName = tl.ContactName,
                                CustomerPhoneNo = tl.CustomerPhoneNo,
                                TaskIssueDate = Convert.ToDateTime(tl.TaskIssueDate),
                                TaskNo = tl.TaskNo,
                                XDSLNo = tl.XDSLNo,
                                TaskStatus = TaskStatusDescription((short)tl.TaskStatus),
                                TaskType = TaskTypeDescription((short)tl.TaskType),
                                ReservationDate = Convert.ToDateTime(tl.ReservationDate),
                                Address = tl.Address,
                                PartnerId = tl.PartnerId,
                                RendezvousTeamStaffName = tl.AssignToRendezvousStaff == null ? null : db.RendezvousTeam.Find(tl.AssignToRendezvousStaff).User.NameSurname,
                                SetupTeamStaffName = tl.AssignToSetupTeam == null ? null : db.SetupTeam.Find(tl.AssignToSetupTeam).User.NameSurname,
                                BBK = tl.BBK,
                            });

                            var totalCount = list.Count();

                            var pagedListByResponseList = new StaticPagedList<GetTaskListResponseViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                            ViewBag.Search = taskListRequestModel;

                            return View(pagedListByResponseList);
                        }
                    }
                    ViewBag.Max30Days = Localization.View.Max30Days;
                    return View();
                }
                ViewBag.StartTimeBiggerThanEndTime = Localization.View.StartDateBiggerThanEndDateError;
                return View();
            }
            ViewBag.ValidationError = "Error";
            return View();



        }

        private List<TaskList> TaskList(bool isAdmin)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();

                if (isAdmin)
                {
                    var adminId = claimInfo.UserId();
                    var adminValid = db.User.Find(adminId);
                    if (adminValid == null)
                    {
                        LoggerError.Fatal("An error occurred while SetupController=>TaskList: Admin not found in User Table");
                    }
                    var taskList = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus != (int)TaskStatusEnum.Completed).ToList();
                    return taskList;
                }
                else
                {
                    var userId = claimInfo.UserId();

                    var taskList = db.TaskList.Where(tl => (tl.AssignToRendezvousStaff == userId || tl.AssignToSetupTeam == userId) && tl.TaskStatus != (int)TaskStatusEnum.Completed && tl.PartnerId == partnerId).ToList();
                    return taskList;
                }
            }
        }

        private string FixedAddress(string address)
        {
            var fixedAddress = address.Replace("..", ".");

            return fixedAddress;
        }

        private string[] GetAddressLatituteandLongitude(string address)
        {
            var parsedAddress = Uri.EscapeDataString(FixedAddress(address));
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key=AIzaSyC2HjBa_I-GX4LOvp71kjtPoZQ4Uz-VBjo&libraries=places&callback=initAutocomplete", parsedAddress);
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xDocument = XDocument.Load(response.GetResponseStream());
            var result = xDocument.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat").Value;
            var lng = locationElement.Element("lng").Value;

            string[] location = { lat, lng };

            return location;
        }

        public ActionResult ShareSetupStaff(string BBK)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            var wrapper = new WebServiceWrapper();
            var serviceAvailability = wrapper.GetApartmentAddress(Convert.ToInt64(BBK));

            using (var db = new PartnerWebSiteEntities())
            {
                var partnerSetupTeamId = db.User.Where(u => u.PartnerId == partnerId).Select(st => st.SetupTeam).Where(st => st.WorkingStatus == true).Select(st => st.UserId).ToList();

                var setupTeamWorkAreas = db.WorkArea.Where(wa => partnerSetupTeamId.Contains(wa.UserId)).ToList();

                var list = new List<deneme>();

                foreach (var item in setupTeamWorkAreas)
                {
                    if (serviceAvailability.AddressDetailsResponse.DistrictID == item.DistrictId &&
                        serviceAvailability.AddressDetailsResponse.ProvinceID == item.ProvinceId &&
                        serviceAvailability.AddressDetailsResponse.RuralCode == item.RuralId &&
                        serviceAvailability.AddressDetailsResponse.NeighbourhoodID == item.NeighbourhoodId
                        )
                    {
                        list.Add(new deneme
                        {
                            ASS = true,
                            Id = item.UserId
                        });

                        //list.Add(item.UserId);
                    }
                    if (serviceAvailability.AddressDetailsResponse.DistrictID == item.DistrictId &&
                                           serviceAvailability.AddressDetailsResponse.ProvinceID == item.ProvinceId &&
                                           serviceAvailability.AddressDetailsResponse.RuralCode == item.RuralId
                                           )
                    {
                        list.Add(new deneme
                        {
                            ASS = list.Count() == 0 ? true : false,
                            Id = item.UserId
                        });
                        //list.Add(item.UserId);
                    }

                    if (serviceAvailability.AddressDetailsResponse.DistrictID == item.DistrictId && serviceAvailability.AddressDetailsResponse.ProvinceID == item.ProvinceId)
                    {
                        list.Add(new deneme
                        {
                            ASS = list.Count() == 0 ? true : false,
                            Id = item.UserId
                        });
                    }

                    if (serviceAvailability.AddressDetailsResponse.ProvinceID == item.ProvinceId)
                    {
                        //list.Add(item.UserId);
                        list.Add(new deneme
                        {
                            ASS = list.Count() == 0 ? true : false,
                            Id = item.UserId
                        });
                    }
                }
                var ass = list;
                var ass2 = "a";
            }



            return View();
        }

        public ActionResult CustomerDetail(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();
            var response = setupWrapper.GetTaskDetails(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var taskDetail = new TaskListDetailResponseViewModel
                {
                    BBK = response.SetupTask.BBK,
                    City = response.SetupTask.City,
                    CustomerNo = response.SetupTask.CustomerNo,
                    Details = response.SetupTask.Details,
                    ModemName = response.SetupTask.ModemName,
                    HasModem = response.SetupTask.HasModem,
                    Province = response.SetupTask.Province,
                    SubscriberNo = response.SetupTask.SubscriberNo,
                    PSTN = response.SetupTask.PSTN,
                    XDSLType = (XDSLTypeEnum)response.SetupTask.XDSLType,
                    CustomerType = CustomerTypeDescription(response.SetupTask.CustomerType),
                    LastConnectionDate = Convert.ToDateTime(response.SetupTask.LastConnectionDate),
                    TaskUpdatesDetailList = response.SetupTask.TaskUpdates == null ? Enumerable.Empty<TaskUpdatesDetailListViewModel>() : response.SetupTask.TaskUpdates.Select(tu => new TaskUpdatesDetailListViewModel
                    {
                        FaultCodes = FaultCodesDescription(tu.FaultCode),
                        Description = tu.Description,
                        CreationDate = Convert.ToDateTime(tu.CreationDate),
                        ReservationDate = Convert.ToDateTime(tu.ReservationDate)
                    }),
                };

                ViewBag.TaskNo = taskNo;
                return View(taskDetail);
            }
            //LOG
            var wrapperGetUserSubMail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetTaskDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperGetUserSubMail.GetUserSubMail()}");
            //LOG

            TempData["GetTaskDetailError"] = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture); ;
            return RedirectToAction("Index", "Setup");
        }

        [HttpPost]
        public ActionResult CustomerSessionInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();
            var response = setupWrapper.GetCustomerSessionInfo(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var sessionInfo = new GetCustomerSessionInfoResponseViewModel()
                {
                    FirstSessionInfo = new SessionInfo()
                    {
                        IPAddress = response.CustomerSessionBundle.FirstSession.IPAddress,
                        IsOnline = response.CustomerSessionBundle.FirstSession.IsOnline,
                        NASIPAddress = response.CustomerSessionBundle.FirstSession.NASIPAddress,
                        SessionId = response.CustomerSessionBundle.FirstSession.SessionId,
                        SessionStart = Convert.ToDateTime(response.CustomerSessionBundle.FirstSession.SessionStart),
                        SessionTime = TimeSpan.TryParse(response.CustomerSessionBundle.FirstSession.SessionTime, out firtSessionTime) == true ? firtSessionTime : TimeSpan.Zero
                    },
                    LastSessionInfo = new SessionInfo()
                    {
                        IPAddress = response.CustomerSessionBundle.LastSession.IPAddress,
                        IsOnline = response.CustomerSessionBundle.LastSession.IsOnline,
                        NASIPAddress = response.CustomerSessionBundle.LastSession.NASIPAddress,
                        SessionId = response.CustomerSessionBundle.LastSession.SessionId,
                        SessionStart = Convert.ToDateTime(response.CustomerSessionBundle.LastSession.SessionStart),
                        SessionTime = TimeSpan.TryParse(response.CustomerSessionBundle.LastSession.SessionTime, out lastSessionTime) == true ? lastSessionTime : TimeSpan.Zero
                    }
                };
                return PartialView("_SessionInfo", sessionInfo);
            }
            //LOG
            var wrapper = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCustomerSessionInfo , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
            //LOG

            return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
        }

        [HttpPost]
        public ActionResult CustomerCredentialsInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();

            var response = setupWrapper.GetCustomerCredentials(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var creadentialsInfo = new GetCustomerCredentialsResponseViewModel
                {
                    Password = response.CustomerCredentials.Password,
                    Username = response.CustomerCredentials.Username
                };

                return PartialView("_CredentialsInfo", creadentialsInfo);
            }
            //LOG
            var wrapperBySubUserMail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCustomerCredentials , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperBySubUserMail.GetUserSubMail()}");
            //LOG
            return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
        }

        [HttpPost]
        public ActionResult CustomerLineInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();

            var response = setupWrapper.GetCustomerLineDetails(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var lineInfo = new GetCustomerLineDetailsViewModel
                {
                    CurrentDowloadSpeed = response.CustomerLineDetails.CurrentDownloadSpeed,
                    CurrentUploadSpeed = response.CustomerLineDetails.CurrentUploadSpeed,
                    DowloadNoiseMargin = response.CustomerLineDetails.DownloadNoiseMargin,
                    DowloadSpeedCapasity = response.CustomerLineDetails.DownloadNoiseMargin,
                    IsActive = response.CustomerLineDetails.IsActive,
                    ShelfCardPort = response.CustomerLineDetails.ShelfCardPort,
                    UploadNoiseMargin = response.CustomerLineDetails.UploadNoiseMargin,
                    UploadSpeedCapasity = response.CustomerLineDetails.UploadSpeedCapacity,
                    XDSLNo = response.CustomerLineDetails.XDSLNo
                };

                return PartialView("_LineInfo", lineInfo);
            }
            //LOG
            var wrapper = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCustomerLineDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
            //LOG
            return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
        }

        [HttpPost]
        public ActionResult CustomerContractInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();

            var response = setupWrapper.GetCustomerContract(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var fileCode = Convert.FromBase64String(response.CustomerContract.FileCode);
                var fileName = response.CustomerContract.FileName;
                return File(fileCode, fileName, fileName);
            }
            //LOG
            var wrapperByGetuserSubmail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCustomerContract , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperByGetuserSubmail.GetUserSubMail()}");
            //LOG

            TempData["CustomerContractResponse"] = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            return RedirectToAction("CustomerDetail", "Setup", new { taskNo = taskNo });
        }

        [HttpGet]
        [Authorize(Roles = "UpdateTaskStatus,Admin")]
        public ActionResult UpdateTaskStatus(long taskNo)
        {
            var request = new AddTaskStatusUpdateViewModel { TaskNo = taskNo };

            ViewBag.FaultTypes = FaultTypeList(null);

            return View(request);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "UpdateTaskStatus,Admin")]
        public ActionResult UpdateTaskStatus(AddTaskStatusUpdateViewModel updateTaskStatusViewModel)
        {
            if (updateTaskStatusViewModel.FaultCodes != FaultCodeEnum.RendezvousMade)
            {
                updateTaskStatusViewModel.ReservationDate = null;
                ModelState.Remove("ReservationDate");
            }

            if (ModelState.IsValid)
            {
                var datetimeNow = DateTime.Now;
                var isValidFaultCodes = Enum.IsDefined(typeof(FaultCodeEnum), updateTaskStatusViewModel.FaultCodes);

                if (isValidFaultCodes)
                {
                    var wrapper = new WebServiceWrapper();
                    using (var db = new PartnerWebSiteEntities())
                    {
                        UpdatedSetupStatus updatedSetupStatus = new UpdatedSetupStatus
                        {
                            ChangeTime = datetimeNow,
                            Description = updateTaskStatusViewModel.Description,
                            FaultCodes = (short)updateTaskStatusViewModel.FaultCodes,
                            ReservationDate = DateTimeConvertedBySetupWebService(updateTaskStatusViewModel.ReservationDate),
                            UserId = db.User.Where(u => u.UserSubMail == wrapper.GetUserSubMail()).FirstOrDefault().Id,
                            TaskNo = (long)updateTaskStatusViewModel.TaskNo,
                        };
                        db.UpdatedSetupStatus.Add(updatedSetupStatus);
                        db.SaveChanges();

                        //LOG
                        Logger.Info("Updated Task Status: " + updateTaskStatusViewModel.TaskNo + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        return RedirectToAction("Successful", new { taskNo = updateTaskStatusViewModel.TaskNo });

                    }
                }
                return RedirectToAction("Index", "Setup");
            }
            ViewBag.FaultTypes = FaultTypeList((int?)updateTaskStatusViewModel.FaultCodes ?? null);
            return View(updateTaskStatusViewModel);
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


        [HttpGet]
        public ActionResult UploadDocument(long taskNo)
        {
            var request = new UploadFileRequestViewModel { TaskNo = taskNo };
            ViewBag.AttachmentTypes = AttachmentTypes(null);

            return View(request);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UploadDocument(UploadFileRequestViewModel uploadFileRequestViewModel, HttpPostedFileBase File)
        {
            ViewBag.AttachmentTypes = AttachmentTypes((int?)uploadFileRequestViewModel.AttachmentTypesEnum ?? null);

            if (ModelState.IsValid)
            {
                var isValidAttachmentType = Enum.IsDefined(typeof(AttachmentTypeEnum), uploadFileRequestViewModel.AttachmentTypesEnum);

                if (isValidAttachmentType)
                {
                    if (File != null && File.ContentLength > 0)
                    {
                        var fileSize = Convert.ToDecimal(File.ContentLength) / 1024 / 1024;

                        if (Properties.Settings.Default.FileSizeLimit > fileSize)
                        {
                            var extension = Path.GetExtension(File.FileName);
                            string[] acceptedExtension = { ".jpg", ".pdf", ".png", ".jpeg" };

                            if (acceptedExtension.Contains(extension))
                            {
                                var fileByte = new byte[File.ContentLength];

                                using (BinaryReader reader = new BinaryReader(File.InputStream))
                                {
                                    fileByte = reader.ReadBytes(File.ContentLength);
                                }

                                var fileData = Convert.ToBase64String(fileByte);

                                var request = new UploadFileRequestViewModel
                                {
                                    AttachmentTypesEnum = uploadFileRequestViewModel.AttachmentTypesEnum,
                                    Extension = extension,
                                    FileData = fileData,
                                    TaskNo = uploadFileRequestViewModel.TaskNo
                                };

                                var setupWrapper = new SetupServiceWrapper();

                                var response = setupWrapper.AddCustomerAttachment(request);

                                if (response.ResponseMessage.ErrorCode == 0)
                                {
                                    //LOG
                                    var wrapper = new WebServiceWrapper();
                                    Logger.Info("Upload Document: " + uploadFileRequestViewModel.TaskNo + ", by: " + wrapper.GetUserSubMail());
                                    //LOG

                                    return RedirectToAction("Successful", "Setup", new { taskNo = uploadFileRequestViewModel.TaskNo });
                                }

                                //LOG
                                var wrapperByGetUserSubmail = new WebServiceWrapper();
                                LoggerError.Fatal($"An error occurred while AddCustomerAttachment , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperByGetUserSubmail.GetUserSubMail()}");
                                //LOG

                                ViewBag.UploadDocumentError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture); ;
                                return View(uploadFileRequestViewModel);

                            }
                            ViewBag.UploadDocumentError = Localization.View.FaultyFormat;
                            return View(uploadFileRequestViewModel);

                        }
                        ViewBag.UploadDocumentError = Localization.View.MaxFileSizeError;
                        return View(uploadFileRequestViewModel);

                    }
                    ViewBag.UploadDocumentError = Localization.View.SelectFile;
                    return View(uploadFileRequestViewModel);

                }
                return RedirectToAction("Index", "Setup");

            }
            return View(uploadFileRequestViewModel);
        }

        [HttpGet]
        public ActionResult UpdateClientLocation(long taskNo)
        {
            var updateGPSViewModel = new UpdateClientGPSRequestViewModel { TaskNo = taskNo };
            return View(updateGPSViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateClientLocation(UpdateClientGPSRequestViewModel updateClientViewModel)
        {
            if (ModelState.IsValid)
            {
                var setupWrapper = new SetupServiceWrapper();

                var response = setupWrapper.UpdateClientLocation(updateClientViewModel);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    //LOG
                    var wrapper = new WebServiceWrapper();
                    Logger.Info("Updated Client User: " + updateClientViewModel.TaskNo + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    return RedirectToAction("Successful", "Setup", new { taskNo = updateClientViewModel.TaskNo });
                }

                //LOG
                var wrapperByGetUserSubmail = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while UpdateClientLocation , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperByGetUserSubmail.GetUserSubMail()}");
                //LOG

                TempData["UpdateGPSResponse"] = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                return RedirectToAction("CustomerDetail", new { taskNo = updateClientViewModel.TaskNo });
            }
            return View(updateClientViewModel);
        }

        public ActionResult Successful(long taskNo)
        {
            ViewBag.TaskNo = taskNo;
            return View();
        }



        private SelectList AttachmentTypes(int? selectedValue)
        {
            var list = new LocalizedList<AttachmentTypeEnum, Localization.AttachmentTypes>().GetList(CultureInfo.CurrentCulture);
            var attachmentTypesList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return attachmentTypesList;
        }

        private SelectList FaultTypeList(int? selectedValue)
        {
            var list = new LocalizedList<FaultCodeEnum, Localization.FaultCodes>().GetList(CultureInfo.CurrentCulture);
            var faultCodesList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return faultCodesList;
        }

        private DateTime ResponseParseDatetime(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return DateTime.MinValue;
            }
            var parsedDate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);
            return parsedDate;
        }

        private string TaskStatusDescription(short value)
        {
            var localizedList = new LocalizedList<TaskStatusEnum, Localization.TaskStatus>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;

        }

        private string CustomerTypeDescription(short value)
        {
            var localizedList = new LocalizedList<CustomerTypeEnum, Localization.CustomerTypes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;
        }

        private string FaultCodesDescription(short value)
        {
            var localizedList = new LocalizedList<FaultCodeEnum, Localization.FaultCodes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;
        }

        private string TaskTypeDescription(short value)
        {
            var localizedList = new LocalizedList<TaskTypesEnum, Localization.TaskTypes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;

        }

        private DateTime ParseDatetime(string date)
        {
            //DateTime dt;
            var convertedDate = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", null);
            //var ass = DateTime.ParseExact(convertedDate, "yyyy-MM-dd HH:mm:ss", null);

            //var asssss = DateTime.TryParseExact(date, "dd.MM.yyyy HH:mm", null, DateTimeStyles., out dt);

            return DateTime.Now;
        }
    }
}