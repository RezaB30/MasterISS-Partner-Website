using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using MasterISS_Partner_WebSite_WebServices.PartnerServiceReference;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    //[Authorize(Roles = "Payment")]
    //[Authorize(Roles = "PaymentManager,Admin")]
    [Authorize]
    public class HomeController : BaseController
    {
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        private string GetPartnerTotalNewSetupTask()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var totalNewSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.New).Count();
                return totalNewSetup.ToString();
            }
        }

        private string GetPartnerTotalCompletedSetupTask()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var totalCompletedSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.Completed).Count();
                return totalCompletedSetup.ToString();
            }
        }

        private string GetPartnerTotalInProgressSetupTask()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var totalInProgressSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.InProgress).Count();
                return totalInProgressSetup.ToString();
            }
        }

        private string CreditReportNotDetail()
        {
            var wrapper = new WebServiceWrapper();
            var response = wrapper.GetCreditReportNotDetail();

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var totalCredit = response.CreditReportResponse.Total;
                return totalCredit.ToString();
            }
            else
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetCreditReport , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                //LOG
                return new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
            }
        }

        private void PartnerInfos()
        {
            ViewBag.TotalInProgressSetup = GetPartnerTotalInProgressSetupTask();
            ViewBag.TotalCompletedSetup = GetPartnerTotalCompletedSetupTask();
            ViewBag.TotalNewSetup = GetPartnerTotalNewSetupTask();
            ViewBag.TotalCredit = CreditReportNotDetail();
        }

        public ActionResult Index()
        {
            ViewBag.Error = "Error";
            PartnerInfos();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Prefix = "search")] GetBillCollectionBySubscriberNoViewModel billListRequestViewModel)
        {
            PartnerInfos();

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

                var details = response.CreditReportResponse.Details.Select(crr => new CreditReportDetailsViewModel
                {
                    Details = crr.Details,
                    Amount = crr.Amount,
                    Date = crr.Date
                });
                detail.CreditReportDetailsViewModel.AddRange(details);

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
            PartnerInfos();

            if (selectedBills != null)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.UserBillList(SubscriberNo);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    var userBillList = response.BillListResponse.Bills.OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => blr.ID);

                    var isMatchedBills = userBillList.SequenceEqual(selectedBills);

                    if (isMatchedBills == false)
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