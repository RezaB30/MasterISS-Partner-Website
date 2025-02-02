﻿using MasterISS_Partner_WebSite_Database.Models;
using NLog;
using RezaB.Scheduling;
using RezaB.Scheduling.StartParameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_Scheduler
{
    public partial class Service1 : ServiceBase
    {
        RezaB.Scheduling.Scheduler scheduler;
        public Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var time = db.SchedulerSettings.Select(ss => ss.ServiceCheckTimer).First();

                    scheduler = new Scheduler(PrepareOperations(time), TimeSpan.FromSeconds(120), "MasterISSPartnerService");
                    if (!scheduler.IsRunning)
                    {
                        scheduler.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerError.Fatal($"An error occurred while OnStart ErrorMessage : { ex.Message}");
            }
        }

        protected override void OnStop()
        {
            scheduler.Stop();
        }
        private static List<SchedulerOperation> PrepareOperations(TimeSpan minute)
        {

            SchedulerTask validTaskStatus = new SchedulerTask("ValidTaskStatus", new ValidTaskStatus());

            SchedulerTask shareUnAssignedTaskToActiveRendezvousTeam = new SchedulerTask("ShareUnAssignedTaskToActiveRendezvousTeam", new ShareUnAssignedTaskToActiveRendezvousTeam());

            var shareList = new List<SchedulerTask>
            {
                shareUnAssignedTaskToActiveRendezvousTeam,
                validTaskStatus
            };

            SchedulerTask getTaskListWebServiceToDatabase = new SchedulerTask("GetTaskListWebServiceToDatabase", new GetTaskListWebServiceToDatabase(), 0, shareList);
            var getTaskList = new List<SchedulerTask>
            {
                getTaskListWebServiceToDatabase
            };

            return new List<SchedulerOperation>()
            {
                new SchedulerOperation("UpdatedTaskStatusDatabaseToWebService",new UpdatedTaskStatusDatabaseToWebService(),new SchedulerTimingOptions(new SchedulerIntervalTimeSpan(minute)),0,getTaskList),
                new SchedulerOperation("TaskUploadedDocumentSendWebService",new TaskUploadedDocumentSendWebService() ,new SchedulerTimingOptions(new SchedulerIntervalTimeSpan(minute)),0),
            };
        }
    }
}
