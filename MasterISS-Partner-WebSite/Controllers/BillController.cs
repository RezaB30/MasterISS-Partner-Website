using MasterISS_Partner_WebSite_WebServices.PartnerServiceReference;
using MasterISS_Partner_WebSite.ViewModels;
using NLog;
using System;
using RezaB.Data.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MasterISS_Partner_WebSite.Enums;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize(Roles = "Payment")]
    [Authorize(Roles = "PaymentManager,Admin")]
    public class BillController : BaseController
    {
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

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

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    if (response.BillListResponse.Bills != null)
                    {
                        ViewBag.Search = billListRequestViewModel;
                        return View("Index", BillList(response));
                    }
                }
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while UserBillList , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                //LOG

                ViewBag.ResponseError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);

            }
            ViewBag.Error = "Error";
            return View("Index");
        }

        [Authorize(Roles = "PaymentCreditReportNotDetail,Admin")]
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

            //LOG
            wrapper = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCreditReport , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
            //LOG

            return Json(new
            {
                errorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture)
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "PaymentCreditReportDetail,Admin")]
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
            //LOG
            wrapper = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while GetCreditReport , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
            //LOG

            ViewBag.ErrorMessage = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            return View();
        }

        public ActionResult Succesfull()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BillOperations(long[] selectedBills, string SubscriberNo)
        {
            if (selectedBills != null)
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
                            //LOG
                            wrapper = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while PayBill , PayBillErrorMessage: {responsePayBill}, by: {wrapper.GetUserSubMail()}");
                            //LOG

                            ViewBag.PayBillError = Localization.BillView.PayBillError;
                        }
                        else
                        {
                            //LOG
                            wrapper = new WebServiceWrapper();
                            Logger.Info($"Customer's bill paid : {SubscriberNo}, BillId: {string.Join(",", selectedBills)} by: {wrapper.GetUserSubMail()}");
                            //LOG

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