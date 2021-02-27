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
    [Authorize(Roles = "Payment")]
    [Authorize(Roles = "PaymentManager,Admin")]
    public class BillController : BaseController
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

                if ( response.ResponseMessage.ErrorCode == 0)
                {
                    if (response.BillListResponse.Bills != null)
                    {
                        ViewBag.Search = billListRequestViewModel;
                        return View("Index", BillList(response));
                    }
                }
                else if (response.ResponseMessage.ErrorCode == 200)
                {
                    ViewBag.ResponseError = Localization.View.GeneralErrorDescription;
                    return View("Index", BillList(response));
                }

                ViewBag.ResponseError = response.ResponseMessage.ErrorMessage;

            }
            ViewBag.Error = "Error";
            return View("Index");
        }

        [Authorize(Roles = "PaymentCreditReportNotDetail")]
        [HttpPost]
        public ActionResult CreditReportNotDetail()
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetCreditReportNotDetail();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var totalCredit = response.CreditReportResponse.Total;
                return Json(new { totalCredit = totalCredit }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { errorMessage = response.ResponseMessage.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "PaymentCreditReportDetail")]
        public ActionResult CreditReportDetail()
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetCreditReportWithDetail();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var detail = new CreditReportViewModel
                {
                    Total = response.CreditReportResponse.Total,
                };

                detail.CreditReportDetailsViewModel = new List<CreditReportDetailsViewModel>();

                foreach (var item in response.CreditReportResponse.Details)
                {
                    detail.CreditReportDetailsViewModel.Add(new CreditReportDetailsViewModel()
                    {
                        Details = item.Details,
                        Amount = item.Amount,
                        Date = item.Date
                    });
                }
                return View(detail);
            }
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                ViewBag.ErrorMessage = Localization.View.GeneralErrorDescription;
                return View();
            }
            ViewBag.ErrorMessage = response.ResponseMessage.ErrorMessage;
            return View();
        }

        public ActionResult Succesfull()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BillOperations(long[] selectedBills, string SubscriberNo)
        {
            if (selectedBills!= null)
            {
                var billListRequestViewModel = new GetBillCollectionBySubscriberNoViewModel()
                {
                    SubscriberNo = SubscriberNo
                };

                var wrapper = new WebServiceWrapper();
                var response = wrapper.UserBillList(SubscriberNo);

                if (response.ResponseMessage.ErrorCode == 0)
                {

                    var userBillList = response.BillListResponse.Bills.OrderBy(blr => blr.IssueDate).Select(blr => blr.ID).ToArray();

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
            return RedirectToAction("Index");
        }
        private BillCollectionViewModel BillList(PartnerServiceBillListResponse response)
        {
            var billList = new BillCollectionViewModel()
            {
                SubscriberName = response.BillListResponse.SubscriberName,
                Bills = response.BillListResponse.Bills == null ? Enumerable.Empty<BillsViewModel>() : response.BillListResponse.Bills.Select(b => new BillsViewModel()
                {
                    IssueDate = b.IssueDate,
                    BillID = b.ID,
                    DueDate = b.DueDate,
                    Cost = b.Total
                })
            };
            return billList;
        }

    }
}