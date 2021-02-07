using MasterISS_Partner_WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class BillController : Controller
    {
        // GET: Bill
        public ActionResult Index()
        {
            var billList =
                new BillCollectionViewModel()
                {
                    SubscriberNo = "8080",
                    Bills = new List<BillsViewModel>()
                        {
                            new BillsViewModel
                            {
                                BillID = "4321432",
                                Cost = "10",
                                DueDate = "2020-12-25 10:00:00",
                                IssueDate = "2020-12-24 10:00:00"
                            },

                            new BillsViewModel
                            {
                                BillID = "123123",
                                Cost = "15",
                                DueDate = "2020-11-25 10:00:00",
                                IssueDate = "2020-11-24 10:00:00"
                            }
                        }
                };
            return View(billList);
        }

        [HttpPost]
        public ActionResult BillOperations(string billList,string subsNo)
        {
            /**/
            /**/
            var identity = User.Identity as ClaimsIdentity;
            var name = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value).SingleOrDefault();
            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                               .Select(c => c.Value).SingleOrDefault(); 

            //var p = identity.Claims.Where(c => c.Type == "UserPassword")
            //                   .Select(c => c.Value).SingleOrDefault();
            /**/
            /**/

            //Get billCollection with subsNo and Control Bills Issue Date

            string[] arrayBill = billList.Split(',');
            List<long> payRequestBillId = new List<long>();
            foreach (var item in arrayBill)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    payRequestBillId.Add(Convert.ToInt64(item));
                }
            }
            //wrapper.payBill(payRequestBillId);

            return View();
        }


    }
}