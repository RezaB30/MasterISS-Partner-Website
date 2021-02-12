using MasterISS_Partner_WebSite.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    //[Authorize(Roles = "Payment")]
    //[Authorize(Roles ="PaymentManager,Admin")]
    public class BillController : Controller
    {
        // GET: Bill
        public ActionResult Index()
        {
            ViewBag.Error = "Error";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Prefix = "search")] GetBillCollectionBySubscriberNoViewModel billListRequestViewModel)
        {
            billListRequestViewModel = billListRequestViewModel ?? new GetBillCollectionBySubscriberNoViewModel();
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.UserBillList(billListRequestViewModel.SubscriberNo);

                if (string.IsNullOrEmpty(response.ErrorMessage) && response.Data.BillListResponse.Bills != null)
                {
                    ViewBag.Search = billListRequestViewModel;
                    return View("Index", BillList(response));
                }
                ViewBag.ResponseError = response.ErrorMessage;

            }
            ViewBag.Error = "Error";
            return View("Index");
        }

        private BillCollectionViewModel BillList(ServiceResponse<PartnerServiceBillListResponse> response)
        {
            var billList = new BillCollectionViewModel()
            {
                SubscriberName = response.Data.BillListResponse.SubscriberName,
                Bills = response.Data.BillListResponse.Bills == null ? Enumerable.Empty<BillsViewModel>() : response.Data.BillListResponse.Bills.Select(b => new BillsViewModel()
                {
                    IssueDate = b.IssueDate,
                    BillID = b.ID,
                    DueDate = b.DueDate,
                    Cost = b.Total
                })
            };
            return billList;
        }

        public ActionResult ass()
        {

            var wrapper2 = new WebServiceWrapper();
            var response2 = wrapper2.GetCreditReportWithDetail();

            //var wrapper = new WebServiceWrapper();
            //var response= wrapper.GetCreditReportNotDetail();

            

            return View();
        }

        public ActionResult Succesfull()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BillOperations(long[] selectedBills, string SubscriberNo)
        {
            var billListRequestViewModel = new GetBillCollectionBySubscriberNoViewModel()
            {
                SubscriberNo = SubscriberNo
            };

            var wrapper = new WebServiceWrapper();
            var response = wrapper.UserBillList(SubscriberNo);

            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                var userBillList = response.Data.BillListResponse.Bills.OrderBy(blr => blr.IssueDate).Select(blr => blr.ID).ToArray();

                var counter = 0;
                for (int i = 0; i < selectedBills.Length; i++)
                {
                    if (userBillList[i] != selectedBills[i])
                    {
                        counter++;
                    }
                }

                if (counter != 0)
                {
                    ViewBag.PayOldBillError = Localization.BillView.PayOldBillError;

                    return View(viewName: "Index", model: BillList(response));
                }

                else
                {
                    wrapper = new WebServiceWrapper();
                    var responsePayBill = wrapper.PayBill(selectedBills);
                    if (!string.IsNullOrEmpty(responsePayBill.ErrorMessage))
                    {
                        ViewBag.PayBillError = Localization.BillView.PayBillError;
                    }
                    else
                    {
                        return RedirectToAction("Succesfull");
                    }
                }
            }
            return View(viewName: "Index", model: BillList(response));
        }


    }
}