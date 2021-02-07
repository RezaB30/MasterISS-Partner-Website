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
            return View();
        }

        [HttpPost]
        public ActionResult BillOperations(string billList,string subsNo)
        {
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