using MasterISS_Partner_WebSite_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Partner_WebSite.ViewModels;
using PagedList;
using NLog;
using RezaB.Data.Localization;
using MasterISS_Partner_WebSite_Enums.Enums;
using System.Globalization;
using RezaB.Data.Files;
using MasterISS_Partner_WebSite_Enums;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class ReportsController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        // GET: Reports

        [Authorize(Roles = "Setup")]
        [Authorize(Roles = "Admin,RendezvousTeam")]
        public ActionResult Index(OperationTypeHistoryFilterViewModel filterViewModel, string isReport, int page = 1, int pageSize = 10)
        {
            filterViewModel = filterViewModel ?? new OperationTypeHistoryFilterViewModel();
            ViewBag.OperationType = OperationTypeList(filterViewModel.OperationType ?? null);
            ViewBag.Search = filterViewModel;

            if (ModelState.IsValid)
            {
                var dateValid = new DatetimeParse();
                if (dateValid.DateIsCorrrect(true, filterViewModel.StartDate, filterViewModel.EndDate))
                {
                    var startDate = dateValid.ConvertDate(filterViewModel.StartDate);
                    var endDate = dateValid.ConvertDate(filterViewModel.EndDate);

                    if (filterViewModel.StartDate != null && filterViewModel.EndDate == null)
                    {
                        endDate = startDate.Value.AddDays(29);
                    }
                    else if ((filterViewModel.StartDate == null && filterViewModel.EndDate == null) || (filterViewModel.StartDate == null && filterViewModel.EndDate != null))
                    {
                        startDate = DateTime.Now.AddDays(-29);
                        endDate = DateTime.Now;
                    }
                    if (startDate <= endDate)
                    {
                        filterViewModel.StartDate = startDate.Value.ToString("dd.MM.yyyy");
                        filterViewModel.EndDate = endDate.Value.ToString("dd.MM.yyyy");

                        if (isReport == Localization.View.GetReport)//GetReport Button
                        {
                            return RedirectToAction("ExportExcelReportForSetup", new { startDate = startDate.GetValueOrDefault().ToString(), endDate = endDate.Value.ToString() });
                        }
                        if (startDate.Value.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                        {
                            using (var db = new PartnerWebSiteEntities())
                            {
                                var operationHistoriesList = OperationHistories(filterViewModel);
                                var list = operationHistoriesList.OrderByDescending(oh => oh.ChangeTime).Select(oh => new OperationHistoryListViewModel
                                {
                                    ChangeTime = oh.ChangeTime,
                                    Description = oh.Description,
                                    OperationType = GetOperationTypeDisplayText(oh.OperationType),
                                    TaskNo = oh.TaskNo,
                                    UserSubMail = GetUserSubMail(oh.UserId),
                                }).ToList();

                                var totalCount = list.Count();

                                var pagedListByResponseList = new StaticPagedList<OperationHistoryListViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                                return View(pagedListByResponseList);
                            }
                        }
                        ViewBag.Max30Days = Localization.View.Max30Days;
                        return View();
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

        public ActionResult ExportExcelReportForSetup(DateTime startDate, DateTime endDate)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            var startDateForFilter = startDate.AddHours(0).AddMinutes(0).AddSeconds(0);
            var endDateForFilter = endDate.AddHours(59).AddMinutes(59).AddSeconds(59);

            using (var db = new PartnerWebSiteEntities())
            {
                var results = db.TaskList.Where(tl => tl.PartnerId == partnerId).Where(tl => tl.TaskIssueDate > startDateForFilter && tl.TaskIssueDate < endDateForFilter).AsEnumerable().Select(tl => new SetupOperationsGenericReportCSVModel
                {
                    Area = string.Format("{0} > {1}", tl.Province, tl.City),
                    CompletionDate = GetTaskComplationDate(tl.TaskNo),
                    Description = GetTaskLastDescriptionByTaskNo(tl.TaskNo),
                    NameSurname = tl.ContactName,
                    SubscriberNo = tl.SubscriberNo,
                    LastRendezvousDate = GetLastReservationDateByTaskNo(tl.TaskNo),
                    SetupTeamStaff = GetSetupStaffNameByUserId(tl.AssignToSetupTeam),
                    TaskIssueDate = tl.TaskIssueDate?.ToString("dd.MM.yyyy HH:mm"),
                    TaskType = GetTaskTypeDisplayText(tl.TaskType),
                    TaskStatus = GetLastTaskStatus(tl.TaskNo),
                    Stage = new LocalizedList<TaskStatusEnum, Localization.TaskStatus>().GetDisplayText(tl.TaskStatus, CultureInfo.CurrentCulture),
                    CustomerStatus = GetCustomerState(tl.SubscriberNo),
                });

                return File(CSVGenerator.GetStream(results, "\t"), @"text/csv", string.Format($"{Localization.View.CurrentTasksReport}_{startDate.ToString("dd.MM.yyyy")}_{endDate.ToString("dd.MM.yyyy")}_.csv"));
            }
        }

        private string GetTaskTypeDisplayText(short? taskTypeId)
        {
            if (taskTypeId != null)
            {
                var taskDisplayText = new LocalizedList<TaskTypesEnum, Localization.TaskTypes>().GetDisplayText(taskTypeId.Value, CultureInfo.CurrentCulture);
                return taskDisplayText;
            }
            return string.Empty;
        }
        private string GetCustomerState(string subscriberNo)
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetSubscriptionState(subscriberNo);
            if (response.ResponseMessage.ErrorCode == 0)
            {
                return response.PartnerSubscriptionState.CustomerState.Name;
            }
            LoggerError.Fatal($"An error occurred while Report=> GetCustomerState , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: { wrapper.GetUserSubMail()}");

            return string.Empty;
        }
        private string GetLastTaskStatus(long taskNo)
        {
            var lastTask = LastUpdatedSetupStatusbyTaskNo(taskNo);
            if (lastTask != null)
            {
                var lastTaskStatus = lastTask.FaultCodes;
                if (lastTaskStatus != null)
                {
                    var lastTaskStatusDisplayText = new LocalizedList<FaultCodeEnum, Localization.FaultCodes>().GetDisplayText(lastTaskStatus, CultureInfo.CurrentCulture);
                    return lastTaskStatusDisplayText;
                }
                return string.Empty;
            }
            return string.Empty;
        }
        private string GetSetupStaffNameByUserId(long? userId)
        {
            if (userId != null)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var user = db.User.Find(userId);
                    if (user != null)
                    {
                        return user.NameSurname;
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }
        private string GetLastReservationDateByTaskNo(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var validTask = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == taskNo);
                if (validTask != null)
                {
                    var lastUpdatedForTask = validTask.OrderByDescending(t => t.ChangeTime).Where(uss => uss.FaultCodes == (short)FaultCodeEnum.RendezvousMade).FirstOrDefault();
                    if (lastUpdatedForTask != null)
                    {
                        if (lastUpdatedForTask.ReservationDate.HasValue)
                        {
                            return lastUpdatedForTask.ReservationDate.GetValueOrDefault().ToString("dd.MM.yyyy HH:mm");
                        }
                    }
                    return null;
                }
                return null;
            }
        }
        private string GetTaskLastDescriptionByTaskNo(long taskNo)
        {
            var lastUpdatedDescription = LastUpdatedSetupStatusbyTaskNo(taskNo);
            if (lastUpdatedDescription != null)
            {
                return lastUpdatedDescription.Description;
            }
            return string.Empty;
        }
        private UpdatedSetupStatus LastUpdatedSetupStatusbyTaskNo(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var validTask = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == taskNo);
                if (validTask != null)
                {
                    var lastUpdatedForTask = validTask.OrderByDescending(t => t.ChangeTime).FirstOrDefault();
                    if (lastUpdatedForTask != null)
                    {
                        return lastUpdatedForTask;
                    }
                    return null;
                }
                return null;
            }
        }
        private string GetTaskComplationDate(long taskNo)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var updatedSetupStatusesbyTaskNo = db.UpdatedSetupStatus.Where(uss => uss.TaskNo == taskNo);
                if (updatedSetupStatusesbyTaskNo.Any())
                {
                    var completedTaskValidation = updatedSetupStatusesbyTaskNo.OrderByDescending(uss => uss.ChangeTime).Where(uss => uss.FaultCodes == (short)FaultCodeEnum.SetupComplete).FirstOrDefault();
                    if (completedTaskValidation != null)
                    {
                        return completedTaskValidation.ChangeTime.ToString("dd.MM.yyyy HH:mm");
                    }
                    return string.Empty;
                }
                return string.Empty;

            }
        }
        private SelectList OperationTypeList(int? selectedValue)
        {
            var list = new LocalizedList<OperationTypeEnum, Localization.OperationHistoryType>().GetList(CultureInfo.CurrentCulture);
            var operationTypeList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);
            return operationTypeList;
        }
        private List<OperationHistory> OperationHistories(OperationTypeHistoryFilterViewModel operationTypeHistoryFilterViewModel)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();

                var operationHistories = Enumerable.Empty<OperationHistory>().AsQueryable();

                var startDate = Convert.ToDateTime(operationTypeHistoryFilterViewModel.StartDate).AddHours(0).AddMinutes(0).AddSeconds(0);
                var endDate = Convert.ToDateTime(operationTypeHistoryFilterViewModel.EndDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                operationHistories = db.OperationHistory.Where(oh => oh.TaskList.PartnerId == partnerId).Where(oh => oh.ChangeTime >= startDate && oh.ChangeTime <= endDate);

                if (operationTypeHistoryFilterViewModel.OperationType != null)
                {
                    var list = operationHistories.Where(oh => oh.OperationType == operationTypeHistoryFilterViewModel.OperationType);
                    operationHistories = list;
                }

                return operationHistories.ToList();
            }
        }
        private string GetOperationTypeDisplayText(short operationType)
        {
            var localizedList = new LocalizedList<OperationTypeEnum, Localization.OperationHistoryType>();
            var displayText = localizedList.GetDisplayText(operationType, CultureInfo.CurrentCulture);
            return displayText;
        }

        private string GetUserSubMail(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.User.Find(userId);
                if (user != null)
                {
                    var userSubMail = user.UserSubMail;
                    return userSubMail;
                }
                else
                {
                    LoggerError.Fatal("An error occurred while ReportsController=>Index: User not found in User Table");
                    return null;

                }
            }
        }
    }
}