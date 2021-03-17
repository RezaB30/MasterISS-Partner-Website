using MasterISS_Partner_WebSite_Database.Models;
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

                    LoggerError.Fatal($"Ass : { time}");

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
            return new List<SchedulerOperation>()
            {
                new SchedulerOperation("GetTaskListWebServiceToDatabase",new GetTaskListWebServiceToDatabase(),new SchedulerTimingOptions(new SchedulerIntervalTimeSpan(minute))),
                new SchedulerOperation("ShareUnAssignedTaskToActiveRendezvousTeam",new ShareUnAssignedTaskToActiveRendezvousTeam(),new SchedulerTimingOptions(new SchedulerIntervalTimeSpan(minute))),
                new SchedulerOperation("UpdatedTaskStatusDatabaseToWebService",new UpdatedTaskStatusDatabaseToWebService(),new SchedulerTimingOptions(new SchedulerIntervalTimeSpan(minute))),
            };
        }
    }
}
