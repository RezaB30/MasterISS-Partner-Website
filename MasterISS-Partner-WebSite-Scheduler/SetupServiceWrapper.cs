using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Scheduler.Enums;
using MasterISS_Partner_WebSite_WebServices.CustomerSetupServiceReference;
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
        private readonly List<WrapperParameters> WrapperParameters;
        private readonly string Culture;
        private readonly string KeyFragment;
        private readonly string Rand;

        private CustomerSetupServiceClient Client { get; set; }

        public SetupServiceWrapper()
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var partnerSetupInfos = db.PartnerSetupInfo.Select(psi => new WrapperParameters()
                {
                    Username = psi.SetupServiceUser,
                    Password = psi.SetupServiceHash,
                });
                WrapperParameters.AddRange(partnerSetupInfos);
            }
            Culture = CultureInfo.CurrentCulture.ToString();
            Rand = Guid.NewGuid().ToString("N");
            Client = new CustomerSetupServiceClient();
        }

        private string CalculateHash<HAT>(string value) where HAT : HashAlgorithm
        {
            HAT algorithm = (HAT)HashAlgorithm.Create(typeof(HAT).Name);
            var calculatedHash = string.Join(string.Empty, algorithm.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(b => b.ToString("x2")));
            return calculatedHash;
        }

        public IEnumerable<GetTaskListResponse> GetTaskList()
        {
            var allGetTaskList = new List<GetTaskListResponse>();

            var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var startDate = DateTime.Now.AddDays(-30);

            using (var db = new PartnerWebSiteEntities())
            {
                var lastGetTaskListLoopTime = db.SchedulerOperationsTime.Where(sot => sot.Type == (int)SchedulerOperationsType.GetTaskList).OrderByDescending(sot => sot.Date).FirstOrDefault();
                if (lastGetTaskListLoopTime != null)
                {
                    startDate = lastGetTaskListLoopTime.Date;
                }

                foreach (var item in WrapperParameters)
                {
                    var request = new GetTaskListRequest
                    {
                        Culture = Culture,
                        Hash = CalculateHash<SHA256>(item.Username + Rand + item.Password + Client.GetKeyFragment(item.Username)),
                        Rand = Rand,
                        Username = item.Username,
                        DateSpan = new DateSpan
                        {
                            EndDate = endDate,
                            StartDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        }
                    };
                    var response = Client.GetTaskList(request);
                    allGetTaskList.Add(response);
                }
                return allGetTaskList;
            }
        }




    }
}
