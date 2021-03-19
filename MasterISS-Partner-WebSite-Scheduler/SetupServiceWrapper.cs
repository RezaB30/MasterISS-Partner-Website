using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using MasterISS_Partner_WebSite_WebServices.CustomerSetupServiceReference;
using NLog;
using RezaB.Scheduling;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                LoggerError.Fatal($"ass2 : { string.Join(",", partnerSetupInfos.Select(p => p.Username))}");


                //WrapperParameters = Enumerable.Empty<WrapperParameters>();
                WrapperParameters = partnerSetupInfos;

                LoggerError.Fatal($"ass9");

            }

            LoggerError.Fatal($"ass10 :{CultureInfo.CurrentCulture}");

            Culture = CultureInfo.CurrentCulture.ToString();
            //Rand = Guid.NewGuid().ToString("N");

            //LoggerError.Fatal($"ass11 : {Guid.NewGuid().ToString("N")}");

            Client = new CustomerSetupServiceClient();
        }

        public string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }
    }
    public class GetTaskListWebServiceToDatabase : AbortableTask
    {
        private SetupServiceWrapper SetupServiceWrapper = new SetupServiceWrapper();

        public void Get()
        {
            try
            {
                SetupServiceWrapper.LoggerError.Fatal($"ASSS1");

                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var startDate = DateTime.Now.AddDays(-30);
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
                                    var taskInfo = db.TaskList.Add(new TaskList
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
                                        LastConnectionDate = taskList.LastConnectionDate,
                                        ModemName = taskList.ModemName,
                                        Province = taskList.Province,
                                        PSTN = taskList.PSTN,
                                        ReservationDate = Convert.ToDateTime(taskList.ReservationDate),
                                        SubscriberNo = taskList.SubscriberNo,
                                        TaskIssueDate = Convert.ToDateTime(taskList.TaskIssueDate),
                                        TaskNo = taskList.TaskNo,
                                        TaskStatus = taskList.TaskStatus,
                                        TaskType = taskList.TaskType,
                                        XDSLNo = taskList.XDSLNo,
                                        XDSLType = taskList.XDSLType,
                                    });
                                    db.TaskList.Add(taskInfo);
                                }
                                SchedulerOperationsTime scheduler = new SchedulerOperationsTime
                                {
                                    Type = (int)SchedulerOperationsType.GetTaskList,
                                    Date = DateTime.Now
                                };
                                db.SchedulerOperationsTime.Add(scheduler);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskList, ErrorMessage:  {ex.Message}");
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
                SetupServiceWrapper.LoggerError.Fatal($"ass4");

                Get();
                SetupServiceWrapper.LoggerError.Fatal($"ass5");

                return true;
            }
            catch (Exception ex)
            {
                SetupServiceWrapper.LoggerError.Fatal($"An error occurred while GetTaskList ErrorMessage : {ex.Message}");
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
                SetupServiceWrapper.LoggerError.Fatal($"ASSS2");

                var dateTimeNow = DateTime.Now;
                var lastChangeTime = DateTime.Now.AddDays(-29);
                using (var db = new PartnerWebSiteEntities())
                {
                    var lastLoopTime = db.SchedulerOperationsTime.Where(sot => sot.Type == (int)SchedulerOperationsType.UpdatedStatusDatabaseToWebService).OrderByDescending(sot => sot.Date).FirstOrDefault();
                    if (lastLoopTime != null)
                    {
                        lastChangeTime = lastLoopTime.Date;
                    }

                    var updatedStatus = db.UpdatedSetupStatus.Where(uss => uss.ChangeTime > lastChangeTime && uss.ChangeTime < dateTimeNow).ToList();

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
                                FaultCode = item.FaultCodes,
                                ReservationDate = item.ReservationDate
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
                SetupServiceWrapper.LoggerError.Fatal($"ASSS3");
                using (var db = new PartnerWebSiteEntities())
                {
                    foreach (var item in SetupServiceWrapper.WrapperParameters)
                    {
                        var partnerRendezvousTeam = db.RendezvousTeam.Where(rt => rt.WorkingStatus == true && rt.User.PartnerId == item.PartnerId && rt.User.IsEnabled).ToList();

                        SetupServiceWrapper.LoggerError.Fatal($"ass12 : {string.Join(" , ", partnerRendezvousTeam.Select(p => p.UserId))}");

                        if (partnerRendezvousTeam != null)
                        {
                            var unAssignedTaskList = db.TaskList.Where(tl => tl.PartnerId == item.PartnerId && tl.AssignToRendezvousStaff == null).ToList();

                            SetupServiceWrapper.LoggerError.Fatal($"ass13 : {string.Join(" , ", unAssignedTaskList.Select(u => u.TaskNo))}");

                            foreach (var task in unAssignedTaskList)
                            {
                                var currentMinHaveTaskRendezvousTeamStaff = partnerRendezvousTeam.Select(staff => new { userId = staff.UserId, taskCount = staff.TaskList.Count }).OrderBy(pt => pt.taskCount).FirstOrDefault();

                                SetupServiceWrapper.LoggerError.Fatal($"ass14 : {string.Join(" , ", currentMinHaveTaskRendezvousTeamStaff)}");

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
}
