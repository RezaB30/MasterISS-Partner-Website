using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using MasterISS_Partner_WebSite_WebServices.CustomerSetupServiceReference;
using NLog;
using RezaB.Scheduling;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_Scheduler
{
    public class SetupServiceWrapper
    {
        public IEnumerable<WrapperParameters> WrapperParameters;
        public string Culture;

        public Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public CustomerSetupServiceClient Client { get; set; }

        public SetupServiceWrapper()
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var partnerSetupInfos = db.PartnerSetupInfo.Select(psi => new WrapperParameters()
                {
                    Username = psi.SetupServiceUser,
                    Password = psi.SetupServiceHash,
                    PartnerId = psi.PartnerId,
                }).ToList();

                WrapperParameters = partnerSetupInfos;
            }

            Culture = CultureInfo.CurrentCulture.ToString();
            Client = new CustomerSetupServiceClient();
        }

        public string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
    }

    public class ValidTaskStatus : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        public void Get()
        {
            try
            {
                var datetimeNow = DateTime.Now;
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var startDate = DateTime.Now.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss"); ;
                using (var db = new PartnerWebSiteEntities())
                {
                    foreach (var item in SetupServiceWrapper.WrapperParameters)
                    {
                        var Rand = Guid.NewGuid().ToString("N");
                        var request = new GetTaskListRequest
                        {
                            Culture = SetupServiceWrapper.Culture,
                            Hash = SetupServiceWrapper.CalculateHash<SHA256>(item.Username + Rand + item.Password + SetupServiceWrapper.Client.GetKeyFragment(item.Username)),
                            Rand = Rand,
                            Username = item.Username,
                            DateSpan = new DateSpan
                            {
                                EndDate = endDate,
                                StartDate = startDate,
                            }
                        };
                        var response = SetupServiceWrapper.Client.GetTaskList(request);

                        if (response.ResponseMessage.ErrorCode != 0)
                        {
                            //LOG
                            SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ValidTaskStatus, ErrorCode:  {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}, PartnerId:{item.PartnerId}");
                            //LOG
                        }
                        else
                        {
                            if (response.SetupTasks.Count() > 0)
                            {
                                var databaseSetupTaskNoList = db.TaskList.Where(tl => tl.PartnerId == item.PartnerId).Select(tl => tl.TaskNo);
                                var webServiceSetupTaskNoList = response.SetupTasks.Select(st => st.TaskNo);

                                var findRemovedTaskList = databaseSetupTaskNoList.Except(webServiceSetupTaskNoList);
                                foreach (var removedTask in findRemovedTaskList)
                                {
                                    var task = db.TaskList.Find(removedTask);
                                    if (task.IsConfirmation == false)
                                    {
                                        task.IsConfirmation = true;
                                    }
                                }

                                foreach (var task in response.SetupTasks.Where(st => st.TaskStatus == (int)TaskStatusEnum.Completed || st.TaskStatus == (int)TaskStatusEnum.Cancelled))
                                {
                                    var dbTask = db.TaskList.Find(task.TaskNo);
                                    if (dbTask != null)
                                    {
                                        if (dbTask.TaskStatus != task.TaskStatus)
                                        {
                                            dbTask.TaskStatus = task.TaskStatus;
                                            var updatedTaskStatus = new UpdatedSetupStatus
                                            {
                                                ChangeTime = datetimeNow,
                                                TaskNo = dbTask.TaskNo,
                                                Description = "Netspeed tarafından değiştirildi",
                                                UserId = null,
                                                ReservationDate = null,
                                                FaultCodes = null,
                                            };
                                            db.UpdatedSetupStatus.Add(updatedTaskStatus);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    SchedulerOperationsTime scheduler = new SchedulerOperationsTime
                    {
                        Type = (short)SchedulerOperationsType.ValidTaskStatus,
                        Date = datetimeNow,
                    };
                    db.SchedulerOperationsTime.Add(scheduler);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbValEx)
            {
                var outputLines = new StringBuilder();
                foreach (var eve in dbValEx.EntityValidationErrors)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ValidTaskStatus GetTaskList, ErrorMessage1:  {eve.Entry.Entity.GetType().Name}{ eve.Entry}");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ValidTaskStatus GetTaskList, ErrorMessage2:  {ve.PropertyName}{ve.ErrorMessage}");
                    }
                }
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ValidTaskStatus GetTaskList, ErrorMessage3:  {outputLines}");
            }
        }

        public override bool Run()
        {
            try
            {
                if (_isAborted)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"ValidTask Aborted");
                    return false;
                }
                Get();
                return true;
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ValidTaskStatus GetTaskList ValidTaskStatus : {ex.Message}");
                return false;
            }
        }
    }

    public class GetTaskListWebServiceToDatabase : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        public void Get()
        {
            try
            {
                var datetimeNow = DateTime.Now;
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var startDate = DateTime.Now.AddDays(-29);
                using (var db = new PartnerWebSiteEntities())
                {
                    var lastGetTaskListLoopTime = db.SchedulerOperationsTime.Where(sot => sot.Type == (int)SchedulerOperationsType.GetTaskList).OrderByDescending(sot => sot.Date).FirstOrDefault();
                    if (lastGetTaskListLoopTime != null)
                    {
                        startDate = lastGetTaskListLoopTime.Date;
                    }

                    foreach (var item in SetupServiceWrapper.WrapperParameters)
                    {
                        var Rand = Guid.NewGuid().ToString("N");
                        var request = new GetTaskListRequest
                        {
                            Culture = SetupServiceWrapper.Culture,
                            Hash = SetupServiceWrapper.CalculateHash<SHA256>(item.Username + Rand + item.Password + SetupServiceWrapper.Client.GetKeyFragment(item.Username)),
                            Rand = Rand,
                            Username = item.Username,
                            DateSpan = new DateSpan
                            {
                                EndDate = endDate,
                                StartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            }
                        };

                        var response = SetupServiceWrapper.Client.GetTaskList(request);

                        if (response.ResponseMessage.ErrorCode != 0)
                        {
                            //LOG
                            SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskList, ErrorCode:  {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}, PartnerId:{item.PartnerId}");
                            //LOG
                        }
                        else
                        {
                            if (response.SetupTasks.Count() > 0)
                            {
                                foreach (var taskList in response.SetupTasks)
                                {
                                    var taskInfo = new TaskList
                                    {
                                        Address = taskList.Address,
                                        PartnerId = item.PartnerId,
                                        AssignToRendezvousStaff = null,
                                        AssignToSetupTeam = null,
                                        BBK = taskList.BBK,
                                        City = taskList.City,
                                        ContactName = taskList.ContactName,
                                        CustomerNo = taskList.CustomerNo,
                                        CustomerPhoneNo = taskList.CustomerPhoneNo,
                                        CustomerType = taskList.CustomerType,
                                        Details = taskList.Details,
                                        HasModem = taskList.HasModem,
                                        LastConnectionDate = ParseDatetime(taskList.LastConnectionDate),
                                        ModemName = taskList.ModemName,
                                        Province = taskList.Province,
                                        PSTN = taskList.PSTN,
                                        ReservationDate = ParseDatetime(taskList.ReservationDate),
                                        SubscriberNo = taskList.SubscriberNo,
                                        TaskIssueDate = ParseDatetime(taskList.TaskIssueDate),
                                        TaskNo = taskList.TaskNo,
                                        TaskStatus = taskList.TaskStatus,
                                        TaskType = taskList.TaskType,
                                        XDSLNo = taskList.XDSLNo,
                                        XDSLType = taskList.XDSLType,
                                        IsConfirmation = false,
                                    };
                                    db.TaskList.Add(taskInfo);
                                }
                            }
                        }
                    }
                    SchedulerOperationsTime scheduler = new SchedulerOperationsTime
                    {
                        Type = (short)SchedulerOperationsType.GetTaskList,
                        Date = datetimeNow,
                    };
                    db.SchedulerOperationsTime.Add(scheduler);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbValEx)
            {
                var outputLines = new StringBuilder();
                foreach (var eve in dbValEx.EntityValidationErrors)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskListWebServiceToDatabase GetTaskList, ErrorMessage1:  {eve.Entry.Entity.GetType().Name}{ eve.Entry}");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskListWebServiceToDatabase GetTaskList, ErrorMessage2:  {ve.PropertyName}{ve.ErrorMessage}");
                    }
                }
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskListWebServiceToDatabase GetTaskList, ErrorMessage3:  {outputLines}");
            }

        }
        private DateTime? ParseDatetime(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            else
            {
                SetupServiceWrapper.LoggerError.Fatal($"stringDate{date}");
                var convertedDate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
                SetupServiceWrapper.LoggerError.Fatal($"convertedDate{convertedDate}");
                return Convert.ToDateTime(convertedDate);
            }
        }
        public override bool Run()
        {
            try
            {
                if (_isAborted)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"Task Aborted");
                    return false;
                }

                Get();

                return true;
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskListWebServiceToDatabase GetTaskList ErrorMessage : {ex.Message}");
                return false;
            }
        }
    }

    public class UpdatedTaskStatusDatabaseToWebService : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        public void Set()
        {
            try
            {
                var dateTimeNow = DateTime.Now;
                var lastChangeTime = DateTime.Now.AddDays(-29);
                using (var db = new PartnerWebSiteEntities())
                {
                    var lastLoopTime = db.SchedulerOperationsTime.Where(sot => sot.Type == (int)SchedulerOperationsType.UpdatedStatusDatabaseToWebService).OrderByDescending(sot => sot.Date).FirstOrDefault();
                    if (lastLoopTime != null)
                    {
                        lastChangeTime = lastLoopTime.Date;
                    }

                    var updatedStatus = db.UpdatedSetupStatus.Where(uss => uss.ChangeTime > lastChangeTime && uss.ChangeTime < dateTimeNow && uss.FaultCodes != null).ToList();

                    foreach (var item in updatedStatus)
                    {
                        var Rand = Guid.NewGuid().ToString("N");
                        var request = new AddTaskStatusUpdateRequest
                        {
                            Culture = SetupServiceWrapper.Culture,
                            Hash = SetupServiceWrapper.CalculateHash<SHA256>(item.TaskList.PartnerSetupInfo.SetupServiceUser + Rand + item.TaskList.PartnerSetupInfo.SetupServiceHash + SetupServiceWrapper.Client.GetKeyFragment(item.TaskList.PartnerSetupInfo.SetupServiceUser)),
                            Rand = Rand,
                            Username = item.TaskList.PartnerSetupInfo.SetupServiceUser,
                            TaskUpdate = new TaskUpdate
                            {
                                TaskNo = item.TaskNo,
                                Description = item.Description,
                                FaultCode = (short)item.FaultCodes,
                                ReservationDate = item.ReservationDate == null ? null : item.ReservationDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                            },
                        };

                        var response = SetupServiceWrapper.Client.AddTaskStatusUpdate(request);
                        if (response.ResponseMessage.ErrorCode != 0)
                        {
                            //LOG
                            SetupServiceWrapper.LoggerError.Fatal($"An error occurred while AddTaskStatusUpdate, ErrorCode:  {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}, PartnerId:{item.TaskList.PartnerSetupInfo.PartnerId}");
                            //LOG
                        }
                        else
                        {
                            SchedulerOperationsTime scheduler = new SchedulerOperationsTime
                            {
                                Type = (int)SchedulerOperationsType.UpdatedStatusDatabaseToWebService,
                                Date = DateTime.Now
                            };
                            db.SchedulerOperationsTime.Add(scheduler);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while AddTaskStatusUpdate, ErrorMessage:  {ex.Message}");
            }
        }

        public override bool Run()
        {
            try
            {
                if (_isAborted)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"Task Aborted");
                    return false;
                }
                Set();
                return true;
            }
            catch (Exception)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while AddTaskStatusUpdate");
                return false;
            }
        }
    }

    public class ShareUnAssignedTaskToActiveRendezvousTeam : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        public void Share()
        {
            try
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    foreach (var item in SetupServiceWrapper.WrapperParameters)
                    {
                        var partnerRendezvousTeam = db.RendezvousTeam.Where(rt => rt.IsAdmin == false && rt.WorkingStatus == true && rt.User.PartnerId == item.PartnerId && rt.User.IsEnabled).ToList();

                        if (partnerRendezvousTeam != null && partnerRendezvousTeam.Count > 0)
                        {
                            var unAssignedTaskList = db.TaskList.Where(tl => tl.PartnerId == item.PartnerId && tl.AssignToRendezvousStaff == null).ToList();

                            foreach (var task in unAssignedTaskList)
                            {
                                var currentMinHaveTaskRendezvousTeamStaff = partnerRendezvousTeam.Select(staff => new { userId = staff.UserId, taskCount = staff.TaskList.Count }).OrderBy(pt => pt.taskCount).FirstOrDefault();

                                task.AssignToRendezvousStaff = currentMinHaveTaskRendezvousTeamStaff.userId;
                                db.SaveChanges();
                            }
                        }
                    }
                    SchedulerOperationsTime scheduler = new SchedulerOperationsTime
                    {
                        Type = (int)SchedulerOperationsType.SharedUnAssignedTask,
                        Date = DateTime.Now
                    };
                    db.SchedulerOperationsTime.Add(scheduler);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ShareUnAssignedTaskToActiveRendezvousTeam, ErrorMessage:  {ex.Message}");

            }
        }

        public override bool Run()
        {
            try
            {
                if (_isAborted)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"Task Aborted");
                    return false;
                }
                Share();
                return true;
            }
            catch (Exception)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while ShareUnAssignedTaskToActiveRendezvousTeam");
                return false;
            }
        }
    }

    public class TaskUploadedDocumentSendWebService : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        private void Send()
        {
            try
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var notSendedTaskForms = db.TaskFormList.Where(tfl => tfl.Status == false).ToList();
                    if (notSendedTaskForms.Count > 0)
                    {
                        foreach (var item in notSendedTaskForms)
                        {
                            var Rand = Guid.NewGuid().ToString("N");

                            var convertedBase64Form = StreamExtensions.GetFileBase64StringValue(item.TaskNo, item.AttachmentType, item.FileName);
                            if (!string.IsNullOrEmpty(convertedBase64Form))
                            {
                                var request = new AddCustomerAttachmentRequest
                                {
                                    Culture = SetupServiceWrapper.Culture,
                                    Hash = SetupServiceWrapper.CalculateHash<SHA256>(item.TaskList.PartnerSetupInfo.SetupServiceUser + Rand + item.TaskList.PartnerSetupInfo.SetupServiceHash + SetupServiceWrapper.Client.GetKeyFragment(item.TaskList.PartnerSetupInfo.SetupServiceUser)),
                                    Rand = Rand,
                                    Username = item.TaskList.PartnerSetupInfo.SetupServiceUser,
                                    CustomerAttachment = new CustomerAttachment
                                    {
                                        FileData = convertedBase64Form,
                                        FileType = new FileInfo(item.FileName).Extension.Replace(".", ""),
                                        TaskNo = item.TaskNo,
                                        AttachmentType = item.AttachmentType
                                    }
                                };

                                var response = SetupServiceWrapper.Client.AddCustomerAttachment(request);

                                if (response.ResponseMessage.ErrorCode == 0)
                                {
                                    item.Status = true;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    SetupServiceWrapper.LoggerError.Fatal($"An error occurred while TaskUploadedDocumentSendWebService, ErrorCode:  {response.ResponseMessage.ErrorCode} , ErrorMessage: {response.ResponseMessage.ErrorMessage}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while TaskUploadedDocumentSendWebService, ErrorMessage:  {ex.Message}");
            }

        }

        public override bool Run()
        {
            try
            {
                if (_isAborted)
                {
                    SetupServiceWrapper.LoggerError.Fatal($"SendForm Aborted");
                    return false;
                }
                Send();
                return true;
            }
            catch (Exception)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while TaskUploadedDocumentSendWebService");
                return false;
            }
        }
    }

}
