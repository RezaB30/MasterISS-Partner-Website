using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite.ViewModels.Revenues;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class RevenuesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SaleAllowedDetails()
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

            //Buraya log ekle error code 0 dan farklı ise

            var list = basicAllowanceDetail.AllowanceDetailsResponse == null ? Enumerable.Empty<GetBasicAllowenceDetailsResponseList>() : basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
            {
                AllowanceStateName = adr.AllowanceStateName,
                Price = adr.Price
            });

            return View(list);
        }

        public ActionResult SetupAllowedDetails()
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

            //Buraya log ekle error code 0 dan farklı ise

            var list = basicAllowanceDetail.AllowanceDetailsResponse == null ? Enumerable.Empty<GetBasicAllowenceDetailsResponseList>() : basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
            {
                AllowanceStateName = adr.AllowanceStateName,
                Price = adr.Price
            });

            return View(list);
        }

        public ActionResult SaleGenericAllowanceList(int page = 0, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                PageNo = page,
                ItemPerPage = pageSize
            };

            var response = wrapper.SaleGenericAllowanceList(request);

            //Buraya log ekle error code 0 dan farklı ise

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

            ViewBag.TotalCount = response.SaleGenericAllowanceList.TotalPageCount;

            return View(list ?? Enumerable.Empty<SaleGenericAllowancesViewModel>());
        }

        public ActionResult SaleAllowanceList(int page = 0, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                ItemPerPage = pageSize,
                PageNo = page,
            };

            var response = wrapper.SaleAllowanceList(request);

            //Buraya log ekle error code 0 dan farklı ise

            var list = response.SaleAllowanceList.SaleAllowances?.Select(sa => new AllowenceListViewModel
            {
                Id = sa.ID,
                Ispaid = sa.IsPaid,
                IssueDate = Convert.ToDateTime(sa.IssueDate),
                PaymentDate = Convert.ToDateTime(sa.PaymentDate),
                Total = sa.Total
            });

            ViewBag.TotalCount = response.SaleAllowanceList.TotalPageCount;

            return View(list ?? Enumerable.Empty<AllowenceListViewModel>());
        }

        public ActionResult SaleAllowanceDetails(int Id, int page = 0, int pageSize = 10)
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

            //Buraya log ekle error code 0 dan farklı ise

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

            ViewBag.TotalCount = response.SaleGenericAllowanceList.TotalPageCount;
            ViewBag.Id = Id;

            return View(list ?? Enumerable.Empty<SaleGenericAllowancesViewModel>());
        }

        public ActionResult SetupGenericAllowanceList(int page = 0, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                PageNo = page,
                ItemPerPage = pageSize
            };

            var response = wrapper.SetupGenericAllowanceList(request);

            //Buraya log ekle error code 0 dan farklı ise

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

            ViewBag.TotalCount = response.SetupGenericAllowanceList.TotalPageCount;

            return View(list ?? Enumerable.Empty<SetupGenericAllowancesViewModel>());
        }

        public ActionResult SetupAllowanceList(int page = 0, int pageSize = 10)
        {
            var wrapper = new WebServiceWrapper();

            var request = new PageSettingsByWebService()
            {
                ItemPerPage = pageSize,
                PageNo = page,
            };

            var response = wrapper.SetupAllowanceList(request);

            ////Buraya log ekle error code 0 dan farklı ise

            var list = response.SetupAllowanceList.SetupAllowances?.Select(sa => new AllowenceListViewModel
            {
                Id = sa.ID,
                Ispaid = sa.IsPaid,
                IssueDate = Convert.ToDateTime(sa.IssueDate),
                PaymentDate = Convert.ToDateTime(sa.PaymentDate),
                Total = sa.Total
            });

            ViewBag.TotalCount = response.SetupAllowanceList.TotalPageCount;

            return View(list ?? Enumerable.Empty<AllowenceListViewModel>());
        }

        public ActionResult SetupAllowenceDetails(int Id, int page = 0, int pageSize = 10)
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

            //Buraya log ekle error code 0 dan farklı ise

            var response = wrapper.SetupAllowanceDetails(request);

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

            ViewBag.TotalCount = response.SetupGenericAllowanceList.TotalPageCount;
            ViewBag.Id = Id;

            return View(list ?? Enumerable.Empty<SetupGenericAllowancesViewModel>());
        }
    }
}