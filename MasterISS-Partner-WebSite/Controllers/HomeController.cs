using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using MasterISS_Partner_WebSite_Enums.Enums;
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
                var details = response.CreditReportResponse.Details.Where(creditDetail => creditDetail.CreditType == (int)PaymentReportTypeEnum.Balance).Select(crr => new CreditReportDetailsViewModel
                {
                    Details = crr.Details,
                    Amount = crr.Amount,
                    Date = crr.Date
                });

                return View(details);
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
        [Authorize(Roles = "PaymentManager,Admin")]
        [Authorize(Roles = "Payment")]
        public ActionResult BillOperations(long[] selectedBills, string SubscriberNo)
        {
            PartnerInfos();

            if (selectedBills != null)
            {
                var wrapper = new WebServiceWrapper();
                var response = wrapper.UserBillList(SubscriberNo);

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    var userBillList = response.BillListResponse.Bills.OrderBy(blr => blr.IssueDate).Take(selectedBills.Length).Select(blr => new { blr.ID, blr.Total });

                    var isMatchedBills = userBillList.Select(ubl => ubl.ID).SequenceEqual(selectedBills);

                    if (isMatchedBills == false)
                    {
                        ViewBag.PayOldBillError = Localization.BillView.PayOldBillError;

                        return View("Index", BillList(response));
                    }

                    var billTotalCount = userBillList.Select(ubl => ubl.Total).Sum();
                    var billList = userBillList.Select(ubl => new UserBillIdAndCost { BillId = ubl.ID, Cost = ubl.Total }).ToArray();
                    Session["BillsSumCount"] = billTotalCount;
                    Session["BillList"] = billList;
                    Session["SubsNo"] = SubscriberNo;
                    Session["SubsName"] = response.BillListResponse.SubscriberName;

                    ViewBag.SumCount = billTotalCount;
                    return View("ConfirmBills");
                }
                return View("Index", BillList(response));
            }
            return RedirectToAction("Index");
        }

        private void RemoveSessionsByBillOperations()
        {
            Session.Remove("BillsSumCount");
            Session.Remove("BillList");
            Session.Remove("SubsNo");
            Session.Remove("SubsName");
        }

        [HttpPost]
        [Authorize(Roles = "PaymentManager,Admin")]
        [Authorize(Roles = "Payment")]
        public ActionResult ConfirmBills()
        {
            var wrapper = new WebServiceWrapper();
            var claimInfo = new ClaimInfo();
            var selectedBills = Session["BillList"] as UserBillIdAndCost[];
            var billSubscriberName = Session["SubsName"].ToString();
            var billSubscriberNo = Session["SubsNo"].ToString();
            if (selectedBills != null && selectedBills.Count() > 0)
            {
                var billsId = selectedBills.Select(sb => sb.BillId).ToArray();
                var responsePayBill = wrapper.PayBill(billsId);


                if (!string.IsNullOrEmpty(responsePayBill.ErrorMessage))
                {
                    //LOG
                    wrapper = new WebServiceWrapper();
                    LoggerError.Fatal($"An error occurred while PayBill , PayBillErrorMessage: {responsePayBill.ErrorMessage}, by: {wrapper.GetUserSubMail()}");
                    //LOG
                    RemoveSessionsByBillOperations();

                    var errorMessage = responsePayBill.ErrorMessage;
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
                        var paidBillList = selectedBills.Select(sb => new PaidBillList
                        {
                            BillCost = sb.Cost,
                            BillId = sb.BillId,
                            ChangeTime = DateTime.Now,
                            PartnerId = claimInfo.PartnerId(),
                            UserId = claimInfo.UserId(),
                            SubscriberName = billSubscriberName,
                            SubscriberNo = billSubscriberNo
                        });

                        db.PaidBillList.AddRange(paidBillList);
                        db.SaveChanges();

                        RemoveSessionsByBillOperations();

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                RemoveSessionsByBillOperations();
                var notDefined = Localization.BillView.PayBillError;
                return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "LastPayments,Admin")]
        public ActionResult LastPayments()
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var lastPaymentList = db.PaidBillList.OrderByDescending(pbl => pbl.ChangeTime).Take(Properties.Settings.Default.LastPaymentsTakeValue).Select(pbl => new LastPaymentListViewModel
                {
                    Amount = pbl.BillCost,
                    PaymentDate = pbl.ChangeTime,
                    SubcriberName = pbl.SubscriberName,
                    SubcriberNo = pbl.SubscriberNo
                }).ToList();

                return View(lastPaymentList);
            }
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
                    Cost = b.Total,
                    BillName = b.ServiceName
                })
            };
            return billList;
        }

        private string GetPartnerTotalNewSetupTask()
        {
            if (User.IsInRole("Setup") && (User.IsInRole("Admin") || User.IsInRole("SetupManager") || User.IsInRole("RendezvousTeam")))
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();

                using (var db = new PartnerWebSiteEntities())
                {
                    var totalNewSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.New).Count();
                    return totalNewSetup.ToString();
                }
            }
            else
            {
                return null;
            }
        }

        private string GetPartnerTotalCompletedSetupTask()
        {
            if (User.IsInRole("Setup") && (User.IsInRole("Admin") || User.IsInRole("SetupManager") || User.IsInRole("RendezvousTeam")))
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();

                using (var db = new PartnerWebSiteEntities())
                {
                    var totalCompletedSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.Completed).Count();
                    return totalCompletedSetup.ToString();
                }
            }
            else
            {
                return null;
            }
        }

        private string GetPartnerTotalInProgressSetupTask()
        {
            if (User.IsInRole("Setup") && (User.IsInRole("Admin") || User.IsInRole("SetupManager") || User.IsInRole("RendezvousTeam")))
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();

                using (var db = new PartnerWebSiteEntities())
                {
                    var totalInProgressSetup = db.TaskList.Where(tl => tl.PartnerId == partnerId && tl.TaskStatus == (short)TaskStatusEnum.InProgress).Count();
                    return totalInProgressSetup.ToString();
                }
            }
            else
            {
                return null;
            }
        }

        private string CreditReportNotDetail()
        {
            if (User.IsInRole("Admin") || User.IsInRole("PaymentCreditReportNotDetail"))
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
            else
            {
                return null;
            }

        }

        private string PartnerTodayPaidBillsSum()
        {
            if (User.IsInRole("Payment") && (User.IsInRole("Admin") || User.IsInRole("PaymentManager")))
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();
                using (var db = new PartnerWebSiteEntities())
                {
                    var total = db.PaidBillList.Where(pbl => pbl.PartnerId == partnerId).Select(pbl => pbl.BillCost).Sum();
                    return total.ToString();
                }
            }
            else
            {
                return null;
            }

        }

        private void PartnerInfos()
        {
            ViewBag.TotalInProgressSetup = GetPartnerTotalInProgressSetupTask();
            ViewBag.TotalCompletedSetup = GetPartnerTotalCompletedSetupTask();
            ViewBag.TotalNewSetup = GetPartnerTotalNewSetupTask();
            ViewBag.TotalCredit = CreditReportNotDetail();
            ViewBag.SumPaidBills = PartnerTodayPaidBillsSum();
        }

    }
}