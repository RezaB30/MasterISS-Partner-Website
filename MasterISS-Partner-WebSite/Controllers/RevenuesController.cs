using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite.ViewModels.Revenues;
using MasterISS_Partner_WebSite_Enums;
using NLog;
using PagedList;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize]
    public class RevenuesController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SaleRevenuesList,Admin")]
        [HttpGet]
        public ActionResult SaleAllowedDetails(int page = 1, int pageSize = 1)
        {
            var wrapper = new WebServiceWrapper();

            var requestByBasicAllowenceDetail = new GetBasicAllowDetailViewModel()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = null,
                    PageNo = null,
                },
                RevenuesTypeEnum = RevenuesTypeEnum.Sale
            };

            var basicAllowanceDetail = wrapper.GetBasicAllowanceDetails(requestByBasicAllowenceDetail);

            if (basicAllowanceDetail.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetBasicAllowanceDetails , ErrorCode: {basicAllowanceDetail.ResponseMessage.ErrorCode}, ErrorMessage : {basicAllowanceDetail.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(basicAllowanceDetail.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = basicAllowanceDetail.AllowanceDetailsResponse == null ? Enumerable.Empty<GetBasicAllowenceDetailsResponseList>() : basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
            {
                AllowanceStateName = adr.AllowanceStateName,
                Price = adr.Price
            });

            var totalCount = list.Count();
            var pagedListByResponseList = new StaticPagedList<GetBasicAllowenceDetailsResponseList>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

            return PartialView("_SaleAllowedDetails", pagedListByResponseList);
        }

        [Authorize(Roles = "SetupRevenuesList,Admin")]
        [HttpGet]
        public ActionResult SetupAllowedDetails(int page = 1, int pageSize = 1)
        {
            var wrapper = new WebServiceWrapper();

            var requestByBasicAllowenceDetail = new GetBasicAllowDetailViewModel()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = null,
                    PageNo = null,
                },
                RevenuesTypeEnum = RevenuesTypeEnum.Setup
            };

            var basicAllowanceDetail = wrapper.GetBasicAllowanceDetails(requestByBasicAllowenceDetail);

            if (basicAllowanceDetail.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while GetBasicAllowanceDetails , ErrorCode: {basicAllowanceDetail.ResponseMessage.ErrorCode}, ErrorMessage : {basicAllowanceDetail.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(basicAllowanceDetail.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = basicAllowanceDetail.AllowanceDetailsResponse == null ? Enumerable.Empty<GetBasicAllowenceDetailsResponseList>() : basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
            {
                AllowanceStateName = adr.AllowanceStateName,
                Price = adr.Price
            });

            var totalCount = list.Count();
            var pagedListByResponseList = new StaticPagedList<GetBasicAllowenceDetailsResponseList>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);

            return PartialView("_SetupAllowedDetails", pagedListByResponseList);
        }


        [HttpGet]
        [Authorize(Roles = "SaleRevenuesList,Admin")]
        public ActionResult SaleAllowanceList(int page = 1, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                ItemPerPage = pageSize,
                PageNo = page,
            };

            var response = wrapper.SaleAllowanceList(request);

            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SaleAllowanceList , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);

            }

            var list = response.SaleAllowanceList.SaleAllowances?.Select(sa => new AllowenceListViewModel
            {
                Id = sa.ID,
                Ispaid = sa.IsPaid,
                IssueDate = Convert.ToDateTime(sa.IssueDate),
                PaymentDate = Convert.ToDateTime(sa.PaymentDate),
                Total = sa.Total
            });

            var totalPageCount = response.SaleAllowanceList.TotalPageCount;
            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<AllowenceListViewModel>(list, page, pageSize, totalItemCount);

            return PartialView("_SaleAllowanceList", pagedListByResponseList);
        }

        [HttpGet]
        [Authorize(Roles = "SaleRevenuesList,Admin")]
        public ActionResult SaleAllowanceDetails(int Id, int page = 1, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new SetupAllowanceDetailsRequest()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = pageSize,
                    PageNo = page
                },
                AllowanceCollectionID = Id
            };

            var response = wrapper.SaleAllowanceDetails(request);

            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SaleAllowanceDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = response.SaleGenericAllowanceList.SaleGenericAllowances?.Select(salega => new SaleGenericAllowancesViewModel
            {
                Allowance = salega.Allowance,
                SubscriptionNo = salega.SubscriptionNo,
                MembershipDate = Convert.ToDateTime(salega.MembershipDate),
                AllowanceState = new AllowanceState
                {
                    Name = salega.AllowanceState.Name,
                    Value = salega.AllowanceState.Value
                },
                SaleState = new State
                {
                    Name = salega.SaleState.Name,
                    Value = salega.SaleState.Value
                },
            });

            var totalPageCount = response.SaleGenericAllowanceList.TotalPageCount;
            ViewBag.Id = Id;

            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<SaleGenericAllowancesViewModel>(list, page, pageSize, totalItemCount);

            return PartialView("_SaleAllowanceDetails", pagedListByResponseList);
        }


        [HttpGet]
        [Authorize(Roles = "SaleRevenuesList,Admin")]
        public ActionResult SaleGenericAllowanceList(int page = 1, int pageSize = 1)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                PageNo = page,
                ItemPerPage = pageSize
            };

            var response = wrapper.SaleGenericAllowanceList(request);

            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SaleGenericAllowanceList , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = response.SaleGenericAllowanceList.SaleGenericAllowances?.Select(salega => new SaleGenericAllowancesViewModel
            {
                Allowance = salega.Allowance,
                SubscriptionNo = salega.SubscriptionNo,
                MembershipDate = Convert.ToDateTime(salega.MembershipDate),
                AllowanceState = new AllowanceState
                {
                    Name = salega.AllowanceState.Name,
                    Value = salega.AllowanceState.Value
                },
                SaleState = new State
                {
                    Name = salega.SaleState.Name,
                    Value = salega.SaleState.Value
                }
            });

            var totalPageCount = response.SaleGenericAllowanceList.TotalPageCount;

            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<SaleGenericAllowancesViewModel>(list, page, pageSize, totalItemCount);


            return PartialView("_SaleGenericAllowanceList", pagedListByResponseList);
        }

        [HttpGet]
        [Authorize(Roles = "SetupRevenuesList,Admin")]
        public ActionResult SetupGenericAllowanceList(int page = 1, int pageSize = 4)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                PageNo = page - 1,
                ItemPerPage = pageSize
            };

            var response = wrapper.SetupGenericAllowanceList(request);


            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SetupGenericAllowanceList , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = response.SetupGenericAllowanceList.SetupGenericAllowances?.Select(setupga => new SetupGenericAllowancesViewModel
            {
                Allowance = setupga.Allowance,
                CompletionDate = Convert.ToDateTime(setupga.CompletionDate),
                AllowanceState = new AllowanceState
                {
                    Name = setupga.AllowanceState.Name,
                    Value = setupga.AllowanceState.Value
                },
                IssueDate = Convert.ToDateTime(setupga.IssueDate),
                SubscriptionNo = setupga.SubscriptionNo,
                SetupState = new State
                {
                    Name = setupga.SetupState.Name,
                    Value = setupga.SetupState.Value
                }
            });

            var totalPageCount = response.SetupGenericAllowanceList.TotalPageCount;
            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<SetupGenericAllowancesViewModel>(list, page, pageSize, totalItemCount);

            return PartialView("_SetupGenericAllowanceList", pagedListByResponseList);
        }

        [HttpGet]
        [Authorize(Roles = "SetupRevenuesList,Admin")]
        public ActionResult SetupAllowanceList(int page = 1, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                ItemPerPage = pageSize,
                PageNo = page,
            };

            var response = wrapper.SetupAllowanceList(request);

            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SetupAllowanceList , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }

            var list = response.SetupAllowanceList.SetupAllowances?.Select(sa => new AllowenceListViewModel
            {
                Id = sa.ID,
                Ispaid = sa.IsPaid,
                IssueDate = Convert.ToDateTime(sa.IssueDate),
                PaymentDate = Convert.ToDateTime(sa.PaymentDate),
                Total = sa.Total
            });

            var totalPageCount = response.SetupAllowanceList.TotalPageCount;
            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<AllowenceListViewModel>(list, page, pageSize, totalItemCount);

            return PartialView("_SetupAllowanceList", pagedListByResponseList);
        }

        [HttpGet]
        [Authorize(Roles = "SetupRevenuesList,Admin")]
        public ActionResult SetupAllowenceDetails(int Id, int page = 1, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new SetupAllowanceDetailsRequest()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = pageSize,
                    PageNo = page
                },
                AllowanceCollectionID = Id
            };

            var response = wrapper.SetupAllowanceDetails(request);

            if (response.ResponseMessage.ErrorCode != 0)
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal($"An error occurred while SetupAllowanceDetails , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage}  by: {wrapper.GetUserSubMail()}");
                //LOG
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture));
                return Content(contect);
            }
            var list = response.SetupGenericAllowanceList.SetupGenericAllowances?.Select(setupga => new SetupGenericAllowancesViewModel
            {
                Allowance = setupga.Allowance,
                AllowanceState = new AllowanceState
                {
                    Name = setupga.AllowanceState.Name,
                    Value = setupga.AllowanceState.Value
                },
                CompletionDate = Convert.ToDateTime(setupga.CompletionDate),
                IssueDate = Convert.ToDateTime(setupga.IssueDate),
                SetupState = new State
                {
                    Name = setupga.SetupState.Name,
                    Value = setupga.SetupState.Value,
                },
                SubscriptionNo = setupga.SubscriptionNo,
            });

            var totalPageCount = response.SetupGenericAllowanceList.TotalPageCount;
            ViewBag.Id = Id;

            var totalItemCount = totalPageCount == 1 ? list.Count() : totalPageCount * pageSize;

            var pagedListByResponseList = new StaticPagedList<SetupGenericAllowancesViewModel>(list, page, pageSize, totalItemCount);

            return View("_SetupAllowenceDetails", pagedListByResponseList);
        }
    }
}