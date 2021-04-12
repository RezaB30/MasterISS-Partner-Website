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
using MasterISS_Partner_WebSite_Enums.Enums;
using System.Net.Http;
using Newtonsoft.Json;

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
        public ActionResult Index([Bind(Prefix = "search")] GetTaskListRequestViewModel taskListRequestModel, int page = 1, int pageSize = 1)
        {
            taskListRequestModel = taskListRequestModel ?? new GetTaskListRequestViewModel();
            ViewBag.TaskType = TaskTypeList(taskListRequestModel.TaskType ?? null);
            ViewBag.FaultCodes = FaultTypeList(taskListRequestModel.FaultCode ?? null, true);
            ViewBag.Search = taskListRequestModel;

            if (ModelState.IsValid)
            {
                var startDate = Convert.ToDateTime(taskListRequestModel.TaskListStartDate);
                var endDate = Convert.ToDateTime(taskListRequestModel.TaskListEndDate);

                if (taskListRequestModel.TaskListStartDate != null && taskListRequestModel.TaskListEndDate == null)
                {
                    endDate = startDate.AddDays(29);
                }
                else if ((taskListRequestModel.TaskListStartDate == null && taskListRequestModel.TaskListEndDate == null) || (taskListRequestModel.TaskListStartDate == null && taskListRequestModel.TaskListEndDate != null))
                {
                    startDate = DateTime.Now.AddDays(-29);
                    endDate = DateTime.Now;
                }

                if (startDate <= endDate)
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
                        if (startDate.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                        {
                            taskListRequestModel.TaskListStartDate = startDate.ToString();
                            taskListRequestModel.TaskListEndDate = endDate.ToString();

                            var taskList = TaskList(User.IsInRole("Admin"), taskListRequestModel);

                            var list = taskList
                                .Select(tl => new GetTaskListResponseViewModel
                                {
                                    AddressLatitudeandLongitude = GetAddressLatituteandLongitude(tl.Address),
                                    ContactName = tl.ContactName,
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
                                    IsCorfirmation = tl.IsConfirmation,
                                    FaultCodesDisplayText = tl.TaskStatus == (int)TaskStatusEnum.New ? null : GetFaultCodesOrDescription(tl.TaskNo, false),
                                    SetupStaffEnteredMessage = tl.TaskStatus == (int)TaskStatusEnum.New ? null : GetFaultCodesOrDescription(tl.TaskNo, true),
                                    TaskStatusByControl = tl.TaskStatus
                                });

                            var totalCount = list.Count();

                            var pagedListByResponseList = new StaticPagedList<GetTaskListResponseViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

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

        private string GetFaultCodesOrDescription(long taskNo, bool getDescription)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var faultCode = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == taskNo).OrderByDescending(uss => uss.ChangeTime).FirstOrDefault();
                if (faultCode == null)
                {
                    return null;
                }

                if (getDescription == false)
                {
                    return FaultCodesDescription(faultCode.FaultCodes);
                }
                else
                {
                    return faultCode.Description;
                }

            }

        }

        public ActionResult CallCustomer(long taskNo)
        {
            //using (var db = new PartnerWebSiteEntities())
            //{
            //    var validTask = db.TaskList.Find(taskNo);
            //    if (validTask != null)
            //    {
            //        var claimInfo = new ClaimInfo();
            //        var sourceNumber = claimInfo.UserPhoneNumber();
            //        var formattedSourceNumber = string.Format("90{0}", sourceNumber);
            //        var customerPhoneNumber = "905387829318";

            //        var Url = string.Format("http://api.bulutsantralim.com/bridge?key=" + Properties.Settings.Default.VerimorKey + "&source={0}&destination={1}", formattedSourceNumber, customerPhoneNumber);


            //        using (var httpClient = new HttpClient())
            //        {
            //            var response = httpClient.GetAsync(Url).Result;
            //            if (response.StatusCode == HttpStatusCode.OK)
            //            {
            //                return RedirectToAction("Index", "Setup");
            //            }
            //            else
            //            {
            //                TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
            //                return RedirectToAction("Index", "Setup");
            //            }

            //        }
            //    }
            return RedirectToAction("Index", "Home");
            //}
        }

        private List<TaskList> TaskList(bool isAdmin, GetTaskListRequestViewModel taskListRequestModel)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();
                var taskList = Enumerable.Empty<TaskList>().AsQueryable();
                var startDate = Convert.ToDateTime(taskListRequestModel.TaskListStartDate);
                var endDate = Convert.ToDateTime(taskListRequestModel.TaskListEndDate);

                if (isAdmin)
                {
                    var adminId = claimInfo.UserId();
                    var adminValid = db.User.Find(adminId);
                    if (adminValid == null)
                    {
                        LoggerError.Fatal("An error occurred while SetupController=>TaskList: Admin not found in User Table");
                    }
                    var searchedValueByContactName = taskListRequestModel.SearchedName ?? "";
                    var list = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.IsConfirmation == false && tl.ContactName.Contains(searchedValueByContactName));
                    taskList = list;
                }
                else
                {
                    var userId = claimInfo.UserId();
                    var searchedValueByContactName = taskListRequestModel.SearchedName ?? "";

                    var list = db.TaskList.Where(tl => (tl.AssignToRendezvousStaff == userId || tl.AssignToSetupTeam == userId) && tl.PartnerId == partnerId && tl.IsConfirmation == false && tl.ContactName.Contains(searchedValueByContactName));
                    taskList = list;
                }

                var listFilterDate = taskList.Where(tlresponse => tlresponse.TaskIssueDate >= startDate&& tlresponse.TaskIssueDate <= endDate);
                taskList = listFilterDate;

                if (taskListRequestModel.SearchedTaskNo != null)
                {
                    var list = taskList.Where(tl => tl.TaskNo == taskListRequestModel.SearchedTaskNo);
                    taskList = list;
                }
                if (taskListRequestModel.TaskType != null)
                {
                    var list = taskList.Where(tl => tl.TaskType == taskListRequestModel.TaskType);
                    taskList = list;
                }
                if (taskListRequestModel.FaultCode != null)
                {
                    var list = taskList.SelectMany(tl => tl.UpdatedSetupStatus.Where(uss => uss.FaultCodes == taskListRequestModel.FaultCode), (tl, uss) => new { taskList = tl }).Select(selectedList => selectedList.taskList);
                    taskList = list;
                }

                return taskList.ToList();
            }
        }

        public ActionResult GiveConfirmation(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var claimInfo = new ClaimInfo();
                    task.IsConfirmation = true;

                    OperationHistory operationHistory = new OperationHistory
                    {
                        ChangeTime = DateTime.Now,
                        Description = new LocalizedList<OperationTypeEnum, Localization.OperationHistoryType>().GetDisplayText((short)OperationTypeEnum.GiveConfirmation, CultureInfo.CurrentCulture),
                        OperationType = (short)OperationTypeEnum.GiveConfirmation,
                        TaskNo = taskNo,
                        UserId = claimInfo.UserId()
                    };
                    db.OperationHistory.Add(operationHistory);
                    db.SaveChanges();

                    return RedirectToAction("Successful");
                }
                else
                {
                    //LOG
                    LoggerError.Fatal("An error occurred while SetupController=>GiveConfirmation: Not Found TaskNo in Task Table ");
                    //LOG

                    TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
                    return RedirectToAction("Index", "Setup");
                }
            }
        }

        //public ActionResult AssignTask(long taskNo, long staffId)
        //{
        //    using (var db = new PartnerWebSiteEntities())
        //    {
        //        var claimInfo = new ClaimInfo();
        //        var wrapper = new WebServiceWrapper();
        //        var task = db.TaskList.Where(tl => tl.TaskNo == taskNo).FirstOrDefault();
        //        if (task != null)
        //        {
        //            var validStaff = db.SetupTeam.Find(staffId);
        //            if (validStaff != null)
        //            {
        //                if (task.AssignToRendezvousStaff == null)
        //                {
        //                    task.AssignToRendezvousStaff = claimInfo.UserId();
        //                }
        //                task.AssignToSetupTeam = staffId;

        //                Logger.Info($"AssignTask TaskNo: {taskNo}, AssignedStaff: {staffId},  by : {claimInfo.UserId()}");

        //                return RedirectToAction("Successful");
        //            }
        //            else
        //            {
        //                LoggerError.Fatal("An error occurred while SetupController=>AssignTask: Not Found Staff in SetupTeam Table ");
        //                TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
        //                return RedirectToAction("Index", "Setup");
        //            }
        //        }
        //        else
        //        {
        //            LoggerError.Fatal("An error occurred while SetupController=>AssignTask: Not Found TaskNo in Task Table ");
        //            TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
        //            return RedirectToAction("Index", "Setup");
        //        }
        //    }
        //}

        public ActionResult ListPartnerRendezvousTeam(long taskNo)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var partnerRendezvousTeam = db.User.Where(rt => rt.PartnerId == partnerId).Select(u => u.RendezvousTeam).Where(rt => rt.WorkingStatus == true && rt.IsAdmin == false).Select(rt => new ListRendezvousTeamViewModel
                    {
                        Id = rt.UserId,
                        NameSurname = rt.User.NameSurname,
                    }).ToArray();

                    ViewBag.TaskNo = taskNo;
                    return PartialView("_ListPartnerRendezvousTeam", partnerRendezvousTeam);
                }
                return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");
            }
        }

        public ActionResult ChangeRendezvousTeam(long rendezvousStaffId, long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var task = db.TaskList.Find(taskNo);
                var validRendezvousTeam = db.RendezvousTeam.Find(rendezvousStaffId);
                if (task != null && validRendezvousTeam != null)
                {
                    task.AssignToRendezvousStaff = rendezvousStaffId;
                    db.SaveChanges();

                    Logger.Info($"Change Rendezvous Team => TaskNo: {taskNo}, ReendezvousTeamSStaffId: {rendezvousStaffId},  by : {claimInfo.UserId()}");

                    return RedirectToAction("Successful");
                }
                else
                {
                    //LOG
                    LoggerError.Fatal("An error occurred while SetupController=>ChangeRendezvousTeam: Not Found TaskNo or Not Found Rendezvous Staff");
                    //LOG

                    TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
                    return RedirectToAction("Index", "Setup");
                }
            }
        }

        [HttpGet]
        public ActionResult SendTaskToScheduler(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var model = new SendTaskToSchedulerViewModel { TaskNo = taskNo, ContactName = task.ContactName };
                    return PartialView("_SendTaskToScheduler", model);
                }
                return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendTaskToScheduler(SendTaskToSchedulerViewModel sendTaskToSchedulerViewModel)
        {
            var claimInfo = new ClaimInfo();
            var wrapper = new WebServiceWrapper();
            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var task = db.TaskList.Find(sendTaskToSchedulerViewModel.TaskNo);
                    if (task != null)
                    {
                        task.AssignToRendezvousStaff = null;
                        task.AssignToSetupTeam = null;
                        task.ReservationDate = null;
                        task.TaskType = (int)TaskTypesEnum.Setup;
                        task.TaskStatus = (int)TaskStatusEnum.New;

                        OperationHistory operationHistory = new OperationHistory
                        {
                            ChangeTime = DateTime.Now,
                            Description = sendTaskToSchedulerViewModel.Description,
                            OperationType = (short)OperationTypeEnum.SentPool,
                            TaskNo = sendTaskToSchedulerViewModel.TaskNo,
                            UserId = claimInfo.UserId()
                        };
                        db.OperationHistory.Add(operationHistory);
                        db.SaveChanges();

                        //Log
                        Logger.Info($"SendSchedulerTask TaskNo: {sendTaskToSchedulerViewModel.TaskNo}, by : {claimInfo.UserId()}");
                        //Log

                        db.SaveChanges();
                        return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        LoggerError.Fatal("An error occurred while SetupController=>SendTaskToScheduler: Not Found TaskNo in Task Table ");

                        return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult CustomerSessionInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();
            var response = setupWrapper.GetCustomerSessionInfo(taskNo);
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
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
                            },
                            CustomerName = task.ContactName,
                        };
                        return PartialView("_SessionInfo", sessionInfo);
                    }
                    //LOG
                    var wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while GetCustomerSessionInfo , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                    //LOG

                    return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
                }

                //LOG
                var wrapperByNotFoundTaskNo = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCustomerSessionInfo Not found taskNo by: {wrapperByNotFoundTaskNo.GetUserSubMail()}");
                //LOG

                return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");
            }
        }

        [HttpPost]
        public ActionResult CustomerDetail(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();
            var response = setupWrapper.GetTaskDetails(taskNo);

            setupWrapper = new SetupServiceWrapper();
            var credentialsResponse = setupWrapper.GetCustomerCredentials(taskNo);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                if (credentialsResponse.ResponseMessage.ErrorCode == 0)
                {
                    var taskDetail = new TaskListDetailResponseViewModel
                    {
                        BBK = response.SetupTask.BBK,
                        CustomerName = response.SetupTask.ContactName,
                        City = response.SetupTask.City,
                        CustomerNo = response.SetupTask.CustomerNo,
                        Details = response.SetupTask.Details,
                        ModemName = response.SetupTask.ModemName,
                        HasModem = response.SetupTask.HasModem,//Modem talebi
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
                        GetCustomerCredentialsResponseViewModel = new GetCustomerCredentialsResponseViewModel
                        {
                            Password = credentialsResponse.CustomerCredentials.Password,
                            Username = credentialsResponse.CustomerCredentials.Username
                        }
                    };
                    return PartialView("_CustomerDetail", taskDetail);
                }

                var wrapperGetUserSubMailByCredentials = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCustomerCredentials , ErrorCode: {credentialsResponse.ResponseMessage.ErrorCode}, ErrorMessage : {credentialsResponse.ResponseMessage.ErrorMessage} by: { wrapperGetUserSubMailByCredentials.GetUserSubMail()}");
                //LOG
                return Content($"<div>{  new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(credentialsResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");

            }
            //LOG
            var wrapperGetUserSubMail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetTaskDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperGetUserSubMail.GetUserSubMail()}");
            //LOG
            return Content($"<div>{  new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
        }

        [HttpPost]
        public ActionResult CustomerLineInfo(long taskNo)
        {
            var setupWrapper = new SetupServiceWrapper();
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
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
                            XDSLNo = response.CustomerLineDetails.XDSLNo,
                            CustomerName = task.ContactName
                        };

                        return PartialView("_LineInfo", lineInfo);
                    }
                    //LOG
                    var wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while GetCustomerLineDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                    //LOG
                    return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)}</div>");
                }
                var wrapperByNotFoundTask = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCustomerLineDetails Not Found taskNo by: {wrapperByNotFoundTask.GetUserSubMail()}");
                //LOG
                return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");

            }
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
            return RedirectToAction("Index", "Setup", new { taskNo = taskNo });
        }

        public ActionResult UpdateTaskStatusNotRendezvous(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var request = new AddTaskStatusUpdateViewModel
                    {
                        TaskNo = taskNo,
                        ContactName = task.ContactName,
                    };
                    var faultTypeList = FaultTypeList(null, false);
                    ViewBag.FaultTypes = faultTypeList;

                    return PartialView("_UpdateTaskStatusNotRendezvous", request);
                }
                else
                {
                    var wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while UpdateTaskStatusNotRendezvous => Get, Task not found, by: {wrapper.GetUserSubMail()}");
                    return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");

                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTaskStatusNotRendezvous(AddTaskStatusUpdateViewModel updateTaskStatusViewModel, HttpPostedFileBase File)
        {
            ModelState.Remove("PostDateValue");
            ModelState.Remove("PostTimeValue");
            ModelState.Remove("StaffId");
            if (ModelState.IsValid)
            {
                if (File != null && File.ContentLength > 0)
                {
                    var fileSize = Convert.ToDecimal(File.ContentLength) / 1024 / 1024;

                    if (Properties.Settings.Default.FileSizeLimit > fileSize)
                    {
                        var extension = Path.GetExtension(File.FileName);
                        string[] acceptedExtension = { ".jpg", ".png", ".jpeg" };

                        if (acceptedExtension.Contains(extension))
                        {
                            var fileOperations = new FileOperations();
                            var saveForm = fileOperations.SaveSetupFile(File.InputStream, File.FileName, updateTaskStatusViewModel.TaskNo.Value);
                            if (saveForm == false)
                            {
                                var claimInfo = new ClaimInfo();

                                LoggerError.Fatal($"An error occurred while UpdateTaskStatusNotRendezvous => Post, Save Form return false, by: {claimInfo.UserId()}");

                                return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = "Failed", ErrorMessage = Localization.View.FaultyFormatUpdateTaskStatus }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = "Failed", ErrorMessage = Localization.View.MaxFileSizeError }, JsonRequestBehavior.AllowGet);
                    }
                }

                using (var db = new PartnerWebSiteEntities())
                {
                    var claimInfo = new ClaimInfo();
                    var wrapper = new WebServiceWrapper();

                    var assignTaskDescription = new LocalizedList<OperationTypeEnum, Localization.OperationHistoryType>().GetDisplayText((short)OperationTypeEnum.AssignTask, CultureInfo.CurrentCulture);

                    OperationHistory operationHistory = new OperationHistory
                    {
                        ChangeTime = DateTime.Now,
                        UserId = claimInfo.UserId(),
                        OperationType = (short)OperationTypeEnum.AssignTask,
                        Description = string.Format($"{assignTaskDescription}, {Localization.View.By}: {wrapper.GetUserSubMail()}"),
                        TaskNo = updateTaskStatusViewModel.TaskNo.Value,
                    };
                    db.OperationHistory.Add(operationHistory);

                    var task = db.TaskList.Find(updateTaskStatusViewModel.TaskNo.Value);
                    if (task == null)
                    {
                        wrapper = new WebServiceWrapper();
                        LoggerError.Fatal($"An error occurred while UpdateTaskStatusNotRendezvous => Post, Task not found, by: {wrapper.GetUserSubMail()}");
                        return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");
                    }
                    task.TaskStatus = (short?)FaultCodeConverter.GetFaultCodeTaskStatus(updateTaskStatusViewModel.FaultCodes);


                    UpdatedSetupStatus updatedSetupStatus = new UpdatedSetupStatus
                    {
                        ChangeTime = DateTime.Now,
                        Description = updateTaskStatusViewModel.Description,
                        FaultCodes = (short)updateTaskStatusViewModel.FaultCodes,
                        ReservationDate = null,
                        UserId = claimInfo.UserId(),
                        TaskNo = (long)updateTaskStatusViewModel.TaskNo,
                    };
                    db.UpdatedSetupStatus.Add(updatedSetupStatus);

                    db.SaveChanges();
                    return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);

                }
            }
            var errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public List<SetupTeamStaffsToMatchedTheTask> ShareSetupStaff(long taskNo)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var wrapperByError = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while ShareSetupStaff , Task not found, by: {wrapperByError.GetUserSubMail()}");
                }

                var wrapper = new WebServiceWrapper();
                var serviceAvailability = wrapper.GetApartmentAddress(Convert.ToInt64(task.BBK));

                var partnerSetupTeamId = db.User.Where(u => u.PartnerId == partnerId).Select(st => st.SetupTeam).Where(st => st.WorkingStatus == true).Select(st => st.UserId).ToList();

                var setupTeamWorkAreas = db.WorkArea.Where(wa => partnerSetupTeamId.Contains(wa.UserId)).ToList();

                var matchedWorkAreaTeamUserId = new List<long>();

                foreach (var setupTeamWorkArea in setupTeamWorkAreas)
                {
                    if (serviceAvailability.AddressDetailsResponse.ProvinceID == setupTeamWorkArea.ProvinceId)
                    {
                        matchedWorkAreaTeamUserId.Add(setupTeamWorkArea.UserId);
                    }

                    if (setupTeamWorkArea.DistrictId.HasValue)
                    {
                        if (serviceAvailability.AddressDetailsResponse.DistrictID != setupTeamWorkArea.DistrictId)
                        {
                            matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                        }
                    }

                    if (setupTeamWorkArea.RuralId.HasValue)
                    {
                        if (serviceAvailability.AddressDetailsResponse.RuralCode != setupTeamWorkArea.RuralId)
                        {
                            matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                        }
                    }

                    if (setupTeamWorkArea.NeighbourhoodId.HasValue)
                    {
                        if (serviceAvailability.AddressDetailsResponse.NeighbourhoodID != setupTeamWorkArea.NeighbourhoodId)
                        {
                            matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                        }
                    }

                }

                var matchedWorkAreaTeam = db.User.Where(wa => matchedWorkAreaTeamUserId.Contains(wa.Id)).Select(user => new SetupTeamStaffsToMatchedTheTask
                {
                    SetupTeamStaffId = user.Id,
                    SetupTeamStaffName = user.NameSurname,
                    SetupTeamStaffWorkAreas = user.WorkArea.Where(matchedWorkArea => matchedWorkArea.UserId == user.Id).Select(wa => new
                    SetupTeamUserAddressInfo()
                    {
                        DistrictName = wa.Districtname,
                        NeigborhoodName = wa.NeighbourhoodName,
                        ProvinceName = wa.ProvinceName,
                        RuralName = wa.RuralName,
                    }).ToList(),
                }).ToList();

                return matchedWorkAreaTeam;

            }
        }
        [HttpPost]
        public ActionResult StaffWorkDays(long staffId)
        {
            var startDate = DateTime.Now.Date;
            var endDate = startDate.AddDays(Properties.Settings.Default.WorkingDaysLong);
            using (var db = new PartnerWebSiteEntities())
            {
                var staff = db.SetupTeam.Find(staffId);
                if (staff == null)
                {
                    var wrapperByNotFoundTask = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while StaffWorkDays by: {wrapperByNotFoundTask.GetUserSubMail()}");

                }
                var staffCalendar = new List<DateTime>();

                var userCurrentWorkDays = StaffCurrentWorkDays(staffId);

                for (DateTime dt = startDate; dt < endDate; dt = dt.AddDays(1))
                {
                    var currentDayOfWeek = (int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek;
                    if (userCurrentWorkDays.Contains(currentDayOfWeek))
                    {
                        staffCalendar.Add(dt);
                    }
                }
                var list = staffCalendar.Select(data => new { Name = data.ToString("dd.MM.yyyy"), Value = data.ToString("dd.MM.yyyy") }).ToArray();
                return Json(new { list = list }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult UpdateTaskStatus(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {

                    var request = new AddTaskStatusUpdateViewModel
                    {
                        TaskNo = taskNo,
                        SetupTeamStaffsToMatchedTheTask = ShareSetupStaff(taskNo),
                        ContactName = task.ContactName
                    };

                    var faultTypeList = FaultTypeList(null, true);

                    ViewBag.FaultTypes = faultTypeList;

                    return PartialView("_UpdateTaskStatus", request);
                }
                else
                {
                    var wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while UpdateTaskStatus => Get, Task not found, by: {wrapper.GetUserSubMail()}");
                    return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");
                }
            }

        }

        [HttpPost]
        public ActionResult UpdateTaskStatus(AddTaskStatusUpdateViewModel updateTaskStatusViewModel)
        {
            if (updateTaskStatusViewModel.FaultCodes != FaultCodeEnum.RendezvousMade)
            {
                updateTaskStatusViewModel.PostDateValue = null;
                updateTaskStatusViewModel.PostTimeValue = null;
                updateTaskStatusViewModel.StaffId = null;
                ModelState.Remove("PostDateValue");
                ModelState.Remove("PostTimeValue");
                ModelState.Remove("StaffId");
            }

            if (ModelState.IsValid)
            {
                var datetimeNow = DateTime.Now;
                var isValidFaultCodes = Enum.IsDefined(typeof(FaultCodeEnum), updateTaskStatusViewModel.FaultCodes);
                var claimInfo = new ClaimInfo();
                var wrapper = new WebServiceWrapper();
                if (isValidFaultCodes)
                {
                    DateTime? reservationDate = null;
                    if (updateTaskStatusViewModel.FaultCodes == FaultCodeEnum.RendezvousMade)
                    {
                        var selectedDate = Convert.ToDateTime(updateTaskStatusViewModel.PostDateValue);
                        var selectedTime = TimeSpan.Parse(updateTaskStatusViewModel.PostTimeValue);
                        reservationDate = selectedDate.Add(selectedTime);

                        var userCurrentWorkDays = StaffCurrentWorkDays(updateTaskStatusViewModel.StaffId.Value);

                        var selectedValueDateOfWeek = (int)selectedDate.DayOfWeek == 0 ? 7 : (int)selectedDate.DayOfWeek;

                        if (!userCurrentWorkDays.Contains(selectedValueDateOfWeek))
                        {
                            return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
                        }
                        var staffWorkTime = StaffWorkTimeCalendar(updateTaskStatusViewModel.StaffId.Value, selectedDate);

                        if (!staffWorkTime.Contains(reservationDate.Value))
                        {
                            return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    using (var db = new PartnerWebSiteEntities())
                    {
                        var validStaff = db.SetupTeam.Find(updateTaskStatusViewModel.StaffId);
                        if (validStaff != null)
                        {
                            var assignTaskDescription = new LocalizedList<OperationTypeEnum, Localization.OperationHistoryType>().GetDisplayText((short)OperationTypeEnum.AssignTask, CultureInfo.CurrentCulture);
                            string description = null;
                            if (updateTaskStatusViewModel.StaffId != null)
                            {
                                description = string.Format($"{assignTaskDescription}, {Localization.View.By}: {wrapper.GetUserSubMail()}, {Localization.View.SetupTeam}: {validStaff.User.UserSubMail} ");
                            }
                            else
                            {
                                description = string.Format($"{assignTaskDescription}, {Localization.View.By}: {wrapper.GetUserSubMail()}");
                            }

                            OperationHistory operationHistory = new OperationHistory
                            {
                                ChangeTime = DateTime.Now,
                                UserId = claimInfo.UserId(),
                                OperationType = (short)OperationTypeEnum.AssignTask,
                                Description = description,
                                TaskNo = updateTaskStatusViewModel.TaskNo.Value,
                            };
                            db.OperationHistory.Add(operationHistory);
                            db.SaveChanges();
                        }
                        else
                        {
                            LoggerError.Fatal($" Setup=>UpdateTaskStatus, Staff Not Found SetupTeam, StaffId: {updateTaskStatusViewModel.StaffId}, Id: {claimInfo.UserId()}");
                            return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
                        }

                        var task = db.TaskList.Find(updateTaskStatusViewModel.TaskNo);

                        UpdatedSetupStatus updatedSetupStatus = new UpdatedSetupStatus
                        {
                            ChangeTime = datetimeNow,
                            Description = updateTaskStatusViewModel.Description,
                            FaultCodes = (short)updateTaskStatusViewModel.FaultCodes,
                            ReservationDate = reservationDate ?? null,
                            UserId = updateTaskStatusViewModel.StaffId == null ? claimInfo.UserId() : updateTaskStatusViewModel.StaffId,
                            TaskNo = (long)updateTaskStatusViewModel.TaskNo,
                        };
                        db.UpdatedSetupStatus.Add(updatedSetupStatus);


                        if (updateTaskStatusViewModel.FaultCodes == FaultCodeEnum.RendezvousMade)
                        {
                            task.AssignToSetupTeam = updateTaskStatusViewModel.StaffId;
                            task.AssignToRendezvousStaff = claimInfo.UserId();
                            task.ReservationDate = reservationDate;
                        }
                        task.TaskStatus = (short?)FaultCodeConverter.GetFaultCodeTaskStatus(updateTaskStatusViewModel.FaultCodes);
                        db.SaveChanges();

                        Logger.Info("Updated Task Status: " + updateTaskStatusViewModel.TaskNo + ", by: " + claimInfo.UserId());
                        return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStaffAvailableHours(string date, long staffId)
        {
            var convertedDate = Convert.ToDateTime(date);
            using (var db = new PartnerWebSiteEntities())
            {
                var staff = db.SetupTeam.Find(staffId);
                if (staff != null)
                {
                    //var list = staffCalendar.Select(data => new { Name = data.ToString("dd.MM.yyyy"), Value = data.ToString("dd.MM.yyyy") }).ToArray();

                    var list = StaffWorkTimeCalendar(staffId, convertedDate).Select(cdl => new { Name = cdl.ToString("HH:mm"), Value = cdl.ToString("HH:mm") }).ToArray();

                    return Json(new { list = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { list = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private IEnumerable<int> StaffCurrentWorkDays(long staffId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var staff = db.SetupTeam.Find(staffId);
                if (staff.WorkDays != null)
                {
                    var userCurrentWorkDays = staff.WorkDays.Split(',').Select(s => Convert.ToInt32(s));
                    return userCurrentWorkDays;
                }
                return Enumerable.Empty<int>();
            }
        }

        private IEnumerable<DateTime> StaffWorkTimeCalendar(long staffId, DateTime date)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var staff = db.SetupTeam.Find(staffId);
                var staffStartTime = staff.WorkStartTime;
                var staffEndTime = staff.WorkEndTime;
                var timeList = new List<TimeSpan>();
                var currentDateList = new List<DateTime>();

                if (staffStartTime == null || staffEndTime == null)
                {
                    return Enumerable.Empty<DateTime>();
                }
                var staffReservationDateList = db.TaskList.Where(tl => tl.AssignToSetupTeam == staffId && tl.TaskStatus != (int)TaskStatusEnum.Completed && tl.ReservationDate.HasValue).Select(rd => rd.ReservationDate).ToList();

                for (TimeSpan tm = staffStartTime.Value; tm < staffEndTime.Value; tm = tm.Add(Properties.Settings.Default.WokingHoursLong))
                {
                    var currentDate = date.Add(tm);
                    currentDateList.Add(currentDate);
                }

                var removedDate = new List<DateTime>();


                for (int i = 0; i < currentDateList.Count - 1; i++)
                {
                    var validStaffReservation = staffReservationDateList.Any(sel => currentDateList.ToArray()[i] <= sel.Value && currentDateList.ToArray()[i + 1] > sel.Value);

                    if (validStaffReservation)
                    {
                        removedDate.Add(currentDateList.ToArray()[i]);
                    }
                }

                foreach (var item in removedDate)
                {
                    currentDateList.Remove(item);
                }

                var filteredDate = currentDateList.Where(s => s < DateTime.Now).ToList();

                foreach (var item in filteredDate)
                {
                    currentDateList.Remove(item);
                }
                return currentDateList;
            }
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
            using (var db = new PartnerWebSiteEntities())
            {
                var task = db.TaskList.Find(taskNo);
                if (task != null)
                {
                    var request = new UploadFileRequestViewModel
                    {
                        TaskNo = taskNo,
                        CustomerName = task.ContactName,
                    };

                    ViewBag.AttachmentTypes = AttachmentTypes(null);

                    return PartialView("_UploadDocument", request);
                }

                var wrapperByNotFoundTask = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while UploadDocument(Get) Not Found taskNo by: {wrapperByNotFoundTask.GetUserSubMail()}");
                //LOG
                return Content($"<div>{ new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture)}</div>");

            }

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
                                var fileOperations = new FileOperations();

                                var saveForm = fileOperations.SaveCustomerForm(File.InputStream, (int)uploadFileRequestViewModel.AttachmentTypesEnum, File.FileName, uploadFileRequestViewModel.TaskNo);
                                if (saveForm == true)
                                {
                                    using (var db = new PartnerWebSiteEntities())
                                    {
                                        TaskFormList taskFormList = new TaskFormList
                                        {
                                            AttachmentType = (short)uploadFileRequestViewModel.AttachmentTypesEnum,
                                            FileName = File.FileName,
                                            Status = false,
                                            TaskNo = uploadFileRequestViewModel.TaskNo,
                                        };
                                        db.TaskFormList.Add(taskFormList);
                                        db.SaveChanges();
                                    }

                                    return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            return Json(new { status = "Failed", ErrorMessage = Localization.View.FaultyFormat }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { status = "Failed", ErrorMessage = Localization.View.MaxFileSizeError }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = "Failed", ErrorMessage = Localization.View.SelectFile }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
            }
            var errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateClientLocation(long taskNo)
        {
            var updateGPSViewModel = new UpdateClientGPSRequestViewModel { TaskNo = taskNo };
            return PartialView("_UpdateClientLocation", updateGPSViewModel);
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

        [HttpPost]
        public ActionResult GetFileList(long taskNo)
        {
            var fileOperations = new FileOperations();

            var taskFileList = fileOperations.GetSetupFileList(taskNo);

            if (taskFileList == null)
            {
                return Json(new { list = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { list = taskFileList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFileTask(string fileName, long taskNo)
        {
            var fileOperations = new FileOperations();
            var getFile = fileOperations.GetFile(taskNo, fileName);

            if (getFile != null)
            {
                return File(getFile, fileName);
            }

            TempData["GeneralError"] = Localization.View.Generic200ErrorCodeMessage;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Successful(long? taskNo)
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


        private SelectList FaultTypeList(int? selectedValue, bool isRendezvous)
        {
            var list = new LocalizedList<FaultCodeEnum, Localization.FaultCodes>().GetList(CultureInfo.CurrentCulture);

            if (!isRendezvous)
            {
                var removedItem = list.Where(l => l.Key == (int)FaultCodeEnum.RendezvousMade).First().Key;
                list.Remove(removedItem);
            }

            var faultCodesList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);

            return faultCodesList;
        }

        private SelectList TaskTypeList(int? selectedValue)
        {
            var list = new LocalizedList<TaskTypesEnum, Localization.TaskTypes>().GetList(CultureInfo.CurrentCulture);
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