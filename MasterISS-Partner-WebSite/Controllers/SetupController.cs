﻿using MasterISS_Partner_WebSite.ViewModels.Setup;
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
    [Authorize(Roles = "Admin,SetupManager,RendezvousTeam")]
    public class SetupController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private TimeSpan firtSessionTime;
        private TimeSpan lastSessionTime;


        public ActionResult Index(GetTaskListRequestViewModel taskListRequestModel, int page = 1, int pageSize = 9)
        {
            taskListRequestModel = taskListRequestModel ?? new GetTaskListRequestViewModel();

            ViewBag.TaskType = TaskTypeList(taskListRequestModel.TaskType ?? null);
            ViewBag.FaultCodes = FaultTypeList(taskListRequestModel.FaultCode ?? null, true);
            //ViewBag.TaskStatus = TaskStatusList(taskListRequestModel.TaskStatus ?? null);
            ViewBag.Search = taskListRequestModel;

            if (ModelState.IsValid)
            {
                var dateValid = new DatetimeParse();
                if (dateValid.DateIsCorrrect(false, taskListRequestModel.TaskListEndDate, taskListRequestModel.TaskListStartDate))
                {
                    var startDate = dateValid.ConvertDateTime(taskListRequestModel.TaskListStartDate);
                    var endDate = dateValid.ConvertDateTime(taskListRequestModel.TaskListEndDate);

                    if (taskListRequestModel.TaskListStartDate != null && taskListRequestModel.TaskListEndDate == null)
                    {
                        endDate = startDate.Value.AddDays(29);
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
                            if (startDate.Value.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                            {
                                taskListRequestModel.TaskListStartDate = startDate.Value.ToString("dd.MM.yyyy HH:mm");
                                taskListRequestModel.TaskListEndDate = endDate.Value.ToString("dd.MM.yyyy HH:mm");

                                var taskList = TaskList(User.IsInRole("Admin"), taskListRequestModel);

                                var list = taskList.Select(tl => new GetTaskListResponseViewModel
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
                                    TaskStatusByControl = tl.TaskStatus,
                                    SubscriberNo = tl.SubscriberNo
                                });

                                var totalCount = list.Count();

                                var pagedListByResponseList = new StaticPagedList<GetTaskListResponseViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                                return View(pagedListByResponseList);

                            }
                            ViewBag.Max30Days = Localization.View.Max30Days;
                            return View();
                        }

                    }
                    ViewBag.StartTimeBiggerThanEndTime = Localization.View.StartDateBiggerThanEndDateError;
                    return View();
                }
                ViewBag.DateFormatIsNotCorrect = Localization.View.DateFormatIsNotCorrect;
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
                    return FaultCodesDescription(faultCode.FaultCodes ?? null);
                }
                else
                {
                    return faultCode.Description;
                }

            }

        }

        private string FormattedNumberPhone(string numberPhone)
        {
            var formattedNumberPhone = string.Format("90{0}", numberPhone);
            return formattedNumberPhone;
        }

        public ActionResult CallCustomer(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var validTask = db.TaskList.Find(taskNo);
                if (validTask != null)
                {
                    var claimInfo = new ClaimInfo();
                    var sourceNumber = claimInfo.UserPhoneNumber();

                    var formattedSourceNumber = FormattedNumberPhone(sourceNumber);
                    var customerPhoneNumber = FormattedNumberPhone(validTask.CustomerPhoneNo);

                    var Url = string.Format("http://api.bulutsantralim.com/bridge?key=" + Properties.Settings.Default.VerimorKey + "&source={0}&destination={1}", formattedSourceNumber, customerPhoneNumber);

                    using (var httpClient = new HttpClient())
                    {
                        var response = httpClient.GetAsync(Url).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var message = Localization.View.Successful;
                            return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);

            }
        }

        private DateTime ConvertDateTime(string date)
        {
            var convertedDate = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture);
            return convertedDate;
        }

        private List<TaskList> TaskList(bool isAdmin, GetTaskListRequestViewModel taskListRequestModel)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();
                var taskList = Enumerable.Empty<TaskList>().AsQueryable();
                var startDate = ConvertDateTime(taskListRequestModel.TaskListStartDate);
                var endDate = ConvertDateTime(taskListRequestModel.TaskListEndDate);
                if (isAdmin)
                {
                    var adminId = claimInfo.UserId();
                    var adminValid = db.User.Find(adminId);
                    if (adminValid == null)
                    {
                        LoggerError.Fatal("An error occurred while SetupController=>TaskList: Admin not found in User Table");
                    }
                    var searchedValueByContactName = taskListRequestModel.SearchedName ?? "";
                    var list = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.IsConfirmation == false && tl.ContactName.Contains(searchedValueByContactName)).OrderByDescending(tl => tl.TaskStatus == (int)TaskStatusEnum.New).ThenBy(tl => tl.TaskStatus == (int)TaskStatusEnum.Completed).ThenBy(tl => tl.TaskStatus == (int)TaskStatusEnum.InProgress);
                    taskList = list;
                }
                else if (User.IsInRole("SetupManager") && !User.IsInRole("RendezvousTeam"))
                {
                    var dateTodayStart = DateTime.Today;
                    var dateTodayEnd = DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var userId = claimInfo.UserId();
                    var searchedValueByContactName = taskListRequestModel.SearchedName ?? "";
                    var list = db.TaskList.Where(tl => tl.AssignToSetupTeam == userId && tl.PartnerId == partnerId && tl.TaskStatus == (int)TaskStatusEnum.InProgress && tl.IsConfirmation == false && tl.ContactName.Contains(searchedValueByContactName)).
                        Where(tl => tl.ReservationDate >= dateTodayStart && tl.ReservationDate <= dateTodayEnd).Where(tl => tl.UpdatedSetupStatus.OrderByDescending(uss => uss.ChangeTime).FirstOrDefault().FaultCodes != (int)FaultCodeEnum.WaitingForNewRendezvous).OrderBy(tl => tl.ReservationDate);

                    taskList = list;
                }
                else
                {
                    var userId = claimInfo.UserId();
                    var searchedValueByContactName = taskListRequestModel.SearchedName ?? "";

                    var list = db.TaskList.Where(tl => (tl.AssignToRendezvousStaff == userId || tl.AssignToSetupTeam == userId) && tl.PartnerId == partnerId && tl.IsConfirmation == false && tl.ContactName.Contains(searchedValueByContactName)).OrderByDescending(tl => tl.TaskStatus == (int)TaskStatusEnum.New).ThenBy(tl => tl.TaskStatus == (int)TaskStatusEnum.Completed).ThenBy(tl => tl.TaskStatus == (int)TaskStatusEnum.InProgress);
                    taskList = list;
                }

                var listFilterDate = taskList.Where(tlresponse => tlresponse.TaskIssueDate >= startDate && tlresponse.TaskIssueDate <= endDate);
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
                    //var list = taskList.SelectMany(tl => tl.UpdatedSetupStatus.Where(uss => uss.FaultCodes == taskListRequestModel.FaultCode), (tl, uss) => new { taskList = tl }).Select(selectedList => selectedList.taskList);
                    var list = taskList.Where(tl => tl.UpdatedSetupStatus.OrderByDescending(uss => uss.ChangeTime).FirstOrDefault().FaultCodes == taskListRequestModel.FaultCode);
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

                    var message = Localization.View.Successful;
                    return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //LOG
                    LoggerError.Fatal("An error occurred while SetupController=>GiveConfirmation: Not Found TaskNo in Task Table ");
                    //LOG
                    var errorMessage = Localization.View.Generic200ErrorCodeMessage;
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
        }

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

        public ActionResult GetUploadedFormsbySubscriberNo(string subscriberNo)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetPartnerClientAttachmentsBySubcriberNo(subscriberNo);
            if (response.ResponseMessage.ErrorCode == 0)
            {
                var fileListViewModel = new GetTasksFileViewModel();

                fileListViewModel.GenericFileList = new List<GenericFileList>();

                foreach (var fileName in response.ClientAttachmentList)
                {
                    var base64 = Convert.ToBase64String(fileName.FileContent);
                    var src = string.Format("data:{0};base64,{1}", fileName.MIMEType, base64);
                    var link = Url.Action("GetSelectedAttachmentbySubscriberNo", "Setup", new { fileName = fileName.FileName, attachmentType = fileName.AttachmentType, subscriberNo = subscriberNo });
                    var genericItem = new GenericFileList
                    {
                        ImgLink = link,
                        ImgSrc = src,
                        AttachmentType = new LocalizedList<AttachmentTypeEnum, Localization.AttachmentTypes>().GetDisplayText(fileName.AttachmentType, CultureInfo.CurrentCulture)
                    };
                    fileListViewModel.GenericFileList.Add(genericItem);
                }

                return PartialView("_GetPartnerClientAttachments", fileListViewModel);
            }

            var errorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", errorMessage, Url.Action("GetPartnerSubscription", "Customer"));
            return Content(contect);
        }

        public ActionResult GetSelectedAttachmentbySubscriberNo(string fileName, int attachmentType, string subscriberNo)
        {
            var wrapper = new WebServiceWrapper();
            var partnerClientAttachments = wrapper.GetPartnerClientAttachmentsBySubcriberNo(subscriberNo);
            if (partnerClientAttachments.ResponseMessage.ErrorCode == 0)
            {
                var file = partnerClientAttachments.ClientAttachmentList.Where(cal => cal.AttachmentType == attachmentType && cal.FileName == fileName).FirstOrDefault();
                if (file != null)
                {
                    return File(file.FileContent, file.FileName);
                }
            }
            TempData["GetSelectedAttachmentErrorMessage"] = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(partnerClientAttachments.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            return RedirectToAction("Index", "Setup");
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
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                return Content(contect);
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

                        //Log
                        Logger.Info($"SendSchedulerTask TaskNo: {sendTaskToSchedulerViewModel.TaskNo}, by : {claimInfo.UserId()}");
                        //Log

                        var removedUpdatedTask = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == sendTaskToSchedulerViewModel.TaskNo).ToList();
                        db.UpdatedSetupStatus.RemoveRange(removedUpdatedTask);
                        db.SaveChanges();

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var notDefined = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        private string FixedAddress(string address)
        {
            var fixedAddress = "";
            fixedAddress = address.Replace("..", ".");
            fixedAddress = fixedAddress.Replace("P.K.: ", "");

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
                    var responseMessage = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture), Url.Action("Index", "Setup"));
                    return Content(responseMessage);
                }

                //LOG
                var wrapperByNotFoundTaskNo = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCustomerSessionInfo Not found taskNo by: {wrapperByNotFoundTaskNo.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                return Content(contect);
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
                var responseMessageCredentials = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", credentialsResponse.ResponseMessage.ErrorMessage, Url.Action("Index", "Setup"));
                return Content(responseMessageCredentials);

            }
            //LOG
            var wrapperGetUserSubMail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetTaskDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperGetUserSubMail.GetUserSubMail()}");
            //LOG
            var responseMessageGetTaskDetail = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", response.ResponseMessage.ErrorMessage, Url.Action("Index", "Setup"));
            return Content(responseMessageGetTaskDetail);
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
                    var responseMessage = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", response.ResponseMessage.ErrorMessage, Url.Action("Index", "Setup"));
                    return Content(responseMessage);
                }
                var wrapperByNotFoundTask = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCustomerLineDetails Not Found taskNo by: {wrapperByNotFoundTask.GetUserSubMail()}");
                //LOG
                var notDefined = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", Localization.View.Generic200ErrorCodeMessage, Url.Action("Index", "Setup"));
                return Content(notDefined);
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

            TempData["CustomerContractResponse"] = response.ResponseMessage.ErrorMessage;
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
                    var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                    return Content(contect);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTaskStatusNotRendezvous(AddTaskStatusUpdateViewModel updateTaskStatusViewModel, IEnumerable<HttpPostedFileBase> files)
        {
            ModelState.Remove("PostDateValue");
            ModelState.Remove("PostTimeValue");
            ModelState.Remove("StaffId");
            if (ModelState.IsValid)
            {
                var fileOperations = new FileOperations();

                if (files != null)
                {
                    var validFiles = fileOperations.ValidFiles(files, false);
                    if (validFiles.Key)
                    {
                        foreach (var file in files)
                        {
                            var saveForm = fileOperations.SaveSetupFile(file.InputStream, file.FileName, updateTaskStatusViewModel.TaskNo.Value);
                            if (saveForm == false)
                            {
                                return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { status = "Failed", ErrorMessage = validFiles.Value }, JsonRequestBehavior.AllowGet);
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

                        var notDefined = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
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
                    var message = Localization.View.Successful;
                    return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);

                }
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public IEnumerable<KeyValuePair<long, string>> ShareSetupStaff(long taskNo)
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


                if (User.IsInRole("Admin") && partnerSetupTeamId.Count == 0)
                {
                    var list = new List<KeyValuePair<long, string>>();

                    var adminId = claimInfo.UserId();
                    var adminName = claimInfo.GetPartnerName();
                    var adminIDAndName = new KeyValuePair<long, string>(adminId, adminName);
                    list.Add(adminIDAndName);
                    return list;
                }

                var setupTeamWorkAreas = db.WorkArea.Where(wa => partnerSetupTeamId.Contains(wa.UserId)).ToList();

                var matchedWorkAreaTeamUserId = new List<long>();

                foreach (var setupTeamWorkArea in setupTeamWorkAreas)
                {
                    if (serviceAvailability.AddressDetailsResponse.ProvinceID == setupTeamWorkArea.ProvinceId)
                    {
                        matchedWorkAreaTeamUserId.Add(setupTeamWorkArea.UserId);

                        if (setupTeamWorkArea.DistrictId.HasValue)
                        {
                            if (serviceAvailability.AddressDetailsResponse.DistrictID != setupTeamWorkArea.DistrictId)
                            {
                                matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                            }
                            else
                            {
                                if (setupTeamWorkArea.RuralId.HasValue)
                                {
                                    if (serviceAvailability.AddressDetailsResponse.RuralCode != setupTeamWorkArea.RuralId)
                                    {
                                        matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                                    }
                                    else
                                    {
                                        if (setupTeamWorkArea.NeighbourhoodId.HasValue)
                                        {
                                            if (serviceAvailability.AddressDetailsResponse.NeighbourhoodID != setupTeamWorkArea.NeighbourhoodId)
                                            {
                                                matchedWorkAreaTeamUserId.Remove(setupTeamWorkArea.UserId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var suitableSetupTeamList = db.User.Where(wa => matchedWorkAreaTeamUserId.Contains(wa.Id)).ToList().Select(user => new KeyValuePair<long, string>(user.Id, user.NameSurname)).ToList();
                return suitableSetupTeamList;
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

        public ActionResult GetSelectedSetupTeamUserInfo(long staffId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.User.Find(staffId);
                if (user != null)
                {

                    var setupTeamInfo = db.User.Where(u => u.Id == staffId).Select(selectedUser => new SetupTeamStaffsToMatchedTheTask
                    {
                        SetupTeamStaffId = selectedUser.Id,
                        SetupTeamStaffName = selectedUser.NameSurname,
                        SetupTeamStaffWorkAreas = selectedUser.WorkArea.Where(matchedWorkArea => matchedWorkArea.UserId == user.Id).Select(wa => new
                        SetupTeamUserAddressInfo()
                        {
                            DistrictName = wa.Districtname,
                            NeigborhoodName = wa.NeighbourhoodName,
                            ProvinceName = wa.ProvinceName,
                            RuralName = wa.RuralName,
                        }).ToList(),
                    }).ToList();

                    return PartialView("_SelectedSetupTeamInfo", setupTeamInfo);
                }
                var responseMessage = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Setup"));
                return Content(responseMessage);
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
                        ContactName = task.ContactName
                    };

                    var faultTypeList = FaultTypeList(null, true);

                    ViewBag.SuitableSetupTeam = SuitableSetupTeamList(taskNo);
                    ViewBag.FaultTypes = faultTypeList;

                    return PartialView("_UpdateTaskStatus", request);
                }
                else
                {
                    var wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while UpdateTaskStatus => Get, Task not found, by: {wrapper.GetUserSubMail()}");

                    var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                    return Content(contect);
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
                        var notDefined = Localization.View.Generic200ErrorCodeMessage;

                        reservationDate = selectedDate.Add(selectedTime);

                        var userCurrentWorkDays = StaffCurrentWorkDays(updateTaskStatusViewModel.StaffId.Value);

                        var selectedValueDateOfWeek = (int)selectedDate.DayOfWeek == 0 ? 7 : (int)selectedDate.DayOfWeek;

                        if (!userCurrentWorkDays.Contains(selectedValueDateOfWeek))
                        {
                            return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                        }
                        var staffWorkTime = StaffWorkTimeCalendar(updateTaskStatusViewModel.StaffId.Value, selectedDate);

                        if (!staffWorkTime.Contains(reservationDate.Value))
                        {
                            return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
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

                                var removedUpdatedTask = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == updateTaskStatusViewModel.TaskNo && uss.FaultCodes == (int)FaultCodeEnum.RendezvousMade).ToList();
                                if (removedUpdatedTask != null)
                                {
                                    db.UpdatedSetupStatus.RemoveRange(removedUpdatedTask);
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
                                Logger.Info("Not Found Staff ; Setup UpdateTaskStatus Post by: " + claimInfo.UserId());

                                var notfoundStaff = Localization.View.Generic200ErrorCodeMessage;
                                return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    using (var db = new PartnerWebSiteEntities())
                    {
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

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

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
                var staffReservationDateList = db.TaskList.Where(tl => tl.AssignToSetupTeam == staffId && tl.TaskStatus != (int)TaskStatusEnum.Completed && tl.ReservationDate.HasValue && tl.IsConfirmation == false).Select(rd => rd.ReservationDate).ToList();

                for (TimeSpan tm = staffStartTime.Value; tm < staffEndTime.Value; tm = tm.Add(Properties.Settings.Default.WokingHoursLong))
                {
                    var currentDate = date.Add(tm);
                    currentDateList.Add(currentDate);
                }

                var removedDate = new List<DateTime>();

                //for (int i = 0; i < currentDateList.Count - 1; i++)
                //{
                //    var validStaffReservation = staffReservationDateList.Any(sel => currentDateList.ToArray()[i] <= sel.Value && currentDateList.ToArray()[i + 1] > sel.Value);

                //    if (validStaffReservation)
                //    {
                //        removedDate.Add(currentDateList.ToArray()[i]);
                //    }
                //}
                for (int i = 0; i < currentDateList.Count - 1; i++)
                {
                    var validStaffReservation = staffReservationDateList.Any(sel => (currentDateList.ToArray()[i] <= sel.Value && currentDateList.ToArray()[i + 1] > sel.Value) || sel.Value == currentDateList.ToArray()[i]);

                    if (staffReservationDateList.Any(sel => sel.Value == currentDateList.ToArray()[i + 1]))
                    {
                        removedDate.Add(currentDateList.ToArray()[i + 1]);
                    }
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
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                return Content(contect);
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UploadDocument(UploadFileRequestViewModel uploadFileRequestViewModel, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var isValidAttachmentType = Enum.IsDefined(typeof(AttachmentTypeEnum), uploadFileRequestViewModel.AttachmentTypesEnum);

                if (isValidAttachmentType)
                {
                    var fileOperations = new FileOperations();
                    var validFiles = fileOperations.ValidFiles(files, true);
                    if (validFiles.Key)
                    {
                        using (var db = new PartnerWebSiteEntities())
                        {
                            foreach (var file in files)
                            {
                                var saveForm = fileOperations.SaveCustomerForm(file.InputStream, (int)uploadFileRequestViewModel.AttachmentTypesEnum, file.FileName, uploadFileRequestViewModel.TaskNo);
                                if (saveForm == true)
                                {
                                    TaskFormList taskFormList = new TaskFormList
                                    {
                                        AttachmentType = (short)uploadFileRequestViewModel.AttachmentTypesEnum,
                                        FileName = file.FileName,
                                        Status = false,
                                        TaskNo = uploadFileRequestViewModel.TaskNo,
                                    };
                                    db.TaskFormList.Add(taskFormList);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var message = Localization.View.Successful;
                            return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { status = "Failed", ErrorMessage = validFiles.Value }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "Failed", ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture) }, JsonRequestBehavior.AllowGet);
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateClientLocation(long taskNo)
        {
            var updateGPSViewModel = new UpdateClientGPSRequestViewModel { TaskNo = taskNo };
            return View("_UpdateClientLocation", updateGPSViewModel);
        }

        [HttpPost]
        public ActionResult UpdateClientLocation(UpdateClientGPSRequestViewModel updateClientViewModel)
        {
            if (ModelState.IsValid)
            {
                if (updateClientViewModel.TaskNo != null)
                {
                    var setupWrapper = new SetupServiceWrapper();

                    var response = setupWrapper.UpdateClientLocation(updateClientViewModel);

                    if (response.ResponseMessage.ErrorCode == 0)
                    {
                        //LOG
                        var wrapper = new WebServiceWrapper();
                        Logger.Info("Updated Client User: " + updateClientViewModel.TaskNo + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }

                    //LOG
                    var wrapperByGetUserSubmail = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while UpdateClientLocation , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperByGetUserSubmail.GetUserSubMail()}");
                    //LOG

                    var responseError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = responseError }, JsonRequestBehavior.AllowGet);
                }
                var notDefined = Localization.View.Generic200ErrorCodeMessage;
                return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetFileList(long taskNo)
        {
            var fileOperations = new FileOperations();

            var taskFileList = fileOperations.GetSetupFileList(taskNo);
            GetTasksFileBase64(taskNo);
            if (taskFileList == null)
            {
                return Json(new { list = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { list = taskFileList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GetTasksFileBase64(long taskNo)
        {
            var fileOperations = new FileOperations();
            var taskFileList = fileOperations.GetSetupFileList(taskNo);
            var fileListViewModel = new GetTasksFileViewModel();
            fileListViewModel.GenericFileList = new List<GenericFileList>();

            if (taskFileList != null)
            {
                foreach (var fileName in taskFileList)
                {
                    var getFile = fileOperations.GetFile(taskNo, fileName);
                    using (var memoryStream = new MemoryStream())
                    {
                        getFile.CopyTo(memoryStream);
                        var bytes = memoryStream.ToArray();
                        var base64 = Convert.ToBase64String(bytes);
                        var src = string.Format("data:image/{0};base64,{1}", new FileInfo(fileName).Extension.Replace(".", ""), base64);
                        var link = Url.Action("GetFileTask", "Setup", new { fileName = fileName, taskNo = taskNo });
                        var genericItem = new GenericFileList
                        {
                            ImgLink = link,
                            ImgSrc = src
                        };
                        fileListViewModel.GenericFileList.Add(genericItem);
                    }
                }
                return PartialView("_GetFileTasks", fileListViewModel);
            }

            return PartialView("_GetFileTasks", fileListViewModel);

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

        private SelectList SuitableSetupTeamList(long taskNo)
        {
            var list = ShareSetupStaff(taskNo);
            if (list.Count() == 0)
            {
                return null;
            }
            var setupTeamList = new SelectList(list.Select(stl => new { Name = stl.Value, Value = stl.Key }).ToArray(), "Value", "name");
            return setupTeamList;
        }

        private SelectList TaskTypeList(int? selectedValue)
        {
            var list = new LocalizedList<TaskTypesEnum, Localization.TaskTypes>().GetList(CultureInfo.CurrentCulture);
            var faultCodesList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return faultCodesList;
        }

        private SelectList TaskStatusList(int? selectedValue)
        {
            var list = new LocalizedList<TaskStatusEnum, Localization.TaskStatus>().GetList(CultureInfo.CurrentCulture);
            var taskStatusList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return taskStatusList;
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

        private string FaultCodesDescription(short? value)
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