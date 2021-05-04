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

namespace MasterISS_Partner_WebSite.Controllers
{
    public class ReportsController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        // GET: Reports
        public ActionResult Index(OperationTypeHistoryFilterViewModel filterViewModel, int page = 1, int pageSize = 10)
        {
            filterViewModel = filterViewModel ?? new OperationTypeHistoryFilterViewModel();
            ViewBag.OperationType = OperationTypeList(filterViewModel.OperationType ?? null);
            ViewBag.Search = filterViewModel;

            if (ModelState.IsValid)
            {
                var dateValid = new DatetimeParse();
                if (dateValid.DateIsCorrrect(false,filterViewModel.StartDate, filterViewModel.EndDate))
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
                        if (startDate.Value.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                        {
                            using (var db = new PartnerWebSiteEntities())
                            {
                                filterViewModel.StartDate = startDate.ToString();
                                filterViewModel.EndDate = endDate.ToString();
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

                var startDate = Convert.ToDateTime(operationTypeHistoryFilterViewModel.StartDate);
                var endDate = Convert.ToDateTime(operationTypeHistoryFilterViewModel.EndDate);

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