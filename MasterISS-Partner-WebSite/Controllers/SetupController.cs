using MasterISS_Partner_WebSite.Enums;
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

namespace MasterISS_Partner_WebSite.Controllers
{
    public class SetupController : BaseController
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
                    XDSLNo = response.SetupTask.XDSLNo,
                    XDSLType = (XDSLTypeEnum)response.SetupTask.XDSLType,
                    TaskStatus = TaskStatusDescription(response.SetupTask.TaskStatus),
                    TaskType = TaskTypeDescription(response.SetupTask.TaskType),
                    CustomerType = CustomerTypeDescription(response.SetupTask.CustomerType),
                    LastConnectionDate = Convert.ToDateTime(response.SetupTask.LastConnectionDate),
                    ReservationDate = Convert.ToDateTime(response.SetupTask.ReservationDate),
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
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                TempData["GetTaskDetailError"] = Localization.View.Generic200ErrorCodeMessage;
                return RedirectToAction("Index", "Setup");
            }
            TempData["GetTaskDetailError"] = response.ResponseMessage.ErrorMessage;
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
                        SessionStart = ResponseParseDatetime(response.CustomerSessionBundle.FirstSession.SessionStart),
                        SessionTime = TimeSpan.Parse(response.CustomerSessionBundle.FirstSession.SessionTime)
                    },
                    LastSessionInfo = new SessionInfo()
                    {
                        IPAddress = response.CustomerSessionBundle.LastSession.IPAddress,
                        IsOnline = response.CustomerSessionBundle.LastSession.IsOnline,
                        NASIPAddress = response.CustomerSessionBundle.LastSession.NASIPAddress,
                        SessionId = response.CustomerSessionBundle.LastSession.SessionId,
                        SessionStart = ResponseParseDatetime(response.CustomerSessionBundle.LastSession.SessionStart),
                        SessionTime = TimeSpan.Parse(response.CustomerSessionBundle.LastSession.SessionTime)
                    }
                };
                return PartialView("_SessionInfo", sessionInfo);
            }
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                return Content($"<div>{Localization.View.Generic200ErrorCodeMessage}</div>");
            }
            return Content($"<div>{response.ResponseMessage.ErrorMessage}</div>");
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
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                return Content($"<div>{Localization.View.Generic200ErrorCodeMessage}</div>");
            }

            return Content($"<div>{response.ResponseMessage.ErrorMessage}</div>");
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
                    //Hız cevaplarında çevirme işlemi yapacak mıyız????
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
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                return Content($"<div>{Localization.View.Generic200ErrorCodeMessage}</div>");
            }
            return Content($"<div>{response.ResponseMessage.ErrorMessage}</div>");
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

            else if (response.ResponseMessage.ErrorCode == 200)
            {
                TempData["CustomerContractResponse"] = Localization.View.Generic200ErrorCodeMessage;
                return RedirectToAction("CustomerDetail", "Setup", new { taskNo = taskNo });
            }

            TempData["CustomerContractResponse"] = response.ResponseMessage.ErrorMessage;
            return RedirectToAction("CustomerDetail", "Setup", new { taskNo = taskNo });
        }

        [HttpGet]
        public ActionResult UpdateTaskStatus(long taskNo)
        {
            var request = new AddTaskStatusUpdateViewModel { TaskNo = taskNo };

            ViewBag.FaultTypes = FaultTypeList(null);

            return View(request);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateTaskStatus(AddTaskStatusUpdateViewModel updateTaskStatusViewModel)
        {
            if (updateTaskStatusViewModel.FaultCodes != FaultCodeEnum.RendezvousMade)
            {
                updateTaskStatusViewModel.ReservationDate = null;
                ModelState.Remove("ReservationDate");
            }

            if (ModelState.IsValid)
            {
                var isValidFaultCodes = Enum.IsDefined(typeof(FaultCodeEnum), updateTaskStatusViewModel.FaultCodes);

                if (isValidFaultCodes)
                {
                    var setupWrapper = new SetupServiceWrapper();

                    var response = setupWrapper.AddTaskStatusUpdate(updateTaskStatusViewModel);

                    if (response.ResponseMessage.ErrorCode == 0)
                    {
                        return RedirectToAction("Successful", new { taskNo = updateTaskStatusViewModel.TaskNo });
                    }

                    TempData["CustomerUpdateStatusError"] = response.ResponseMessage.ErrorMessage;
                    return RedirectToAction("CustomerDetail", new { taskNo = updateTaskStatusViewModel.TaskNo });
                }
                return RedirectToAction("Index", "Setup");
            }
            ViewBag.FaultTypes = FaultTypeList((int?)updateTaskStatusViewModel.FaultCodes ?? null);
            return View(updateTaskStatusViewModel);
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
                                    return RedirectToAction("Successful", "Setup", new { taskNo = uploadFileRequestViewModel.TaskNo });
                                }

                                ViewBag.UploadDocumentError = response.ResponseMessage.ErrorMessage;
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
                    return RedirectToAction("Successful", "Setup", new { taskNo = updateClientViewModel.TaskNo });
                }
                TempData["UpdateGPSResponse"] = response.ResponseMessage.ErrorMessage;
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

        private string TaskStatusDescription(int value)
        {
            var localizedList = new LocalizedList<TaskStatusEnum, Localization.TaskStatus>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;

        }

        private string CustomerTypeDescription(int value)
        {
            var localizedList = new LocalizedList<CustomerTypeEnum, Localization.CustomerTypes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;
        }

        private string FaultCodesDescription(int value)
        {
            var localizedList = new LocalizedList<FaultCodeEnum, Localization.FaultCodes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;
        }

        private string TaskTypeDescription(int value)
        {
            var localizedList = new LocalizedList<TaskTypesEnum, Localization.TaskTypes>();
            var displayText = localizedList.GetDisplayText(value, CultureInfo.CurrentCulture);

            return displayText;

        }

        private DateTime ParseDatetime(string date)
        {
            var convertedDate = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", null);

            return convertedDate;
        }
    }
}