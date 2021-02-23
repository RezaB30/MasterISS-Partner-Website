﻿using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite.ViewModels.Setup;
using MasterISS_Partner_WebSite.ViewModels.Setup.Response;
using PagedList;
using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup
        public ActionResult Index([Bind(Prefix = "search")] GetTaskListRequestViewModel taskListRequestModel, int page = 1, int pageSize = 20)
        {
            taskListRequestModel = taskListRequestModel ?? new GetTaskListRequestViewModel();

            if (string.IsNullOrEmpty(taskListRequestModel.TaskListStartDate) || string.IsNullOrEmpty(taskListRequestModel.TaskListEndDate))
            {
                taskListRequestModel.TaskListStartDate = DateTime.Now.AddDays(-30).ToString("dd.MM.yyyy HH:mm");
                taskListRequestModel.TaskListEndDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            }
            if (ModelState.IsValid)
            {
                var startDate = ParseDatetime(taskListRequestModel.TaskListStartDate);
                var endDate = ParseDatetime(taskListRequestModel.TaskListEndDate);

                if (startDate <= endDate)
                {
                    if (startDate.AddDays(Properties.Settings.Default.SearchLimit) >= endDate)
                    {
                        var setupWrapper = new SetupServiceWrapper();

                        var response = setupWrapper.GetTaskList(taskListRequestModel);

                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            var list = response.SetupTasks.Where(tlresponse => tlresponse.ContactName.Contains(taskListRequestModel.SearchedName ?? "") && tlresponse.TaskStatus != (int)TaskStatusEnum.Completed).Select(st => new GetTaskListResponseViewModel
                            {
                                Address = st.Address,
                                ContactName = st.ContactName,
                                CustomerPhoneNo = st.CustomerPhoneNo,
                                TaskIssueDate = Convert.ToDateTime(st.TaskIssueDate),
                                TaskNo = st.TaskNo,
                            });

                            var totalCount = list.Count();

                            var pagedListByResponseList = new StaticPagedList<GetTaskListResponseViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

                            ViewBag.Search = taskListRequestModel;

                            return View(pagedListByResponseList);
                        }
                        ViewBag.ErrorMessage = Localization.View.SetupResponseErrorMessage;
                        return View();
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


        private DateTime ParseDatetime(string date)
        {
            var convertedDate = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", null);

            return convertedDate;
        }
    }
}