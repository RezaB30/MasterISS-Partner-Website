using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class RevenuesController : Controller
    {
        // GET: Revenues
        public ActionResult SaleAllowedDetails()
        {
            var wrapper = new WebServiceWrapper();

            var requestByBasicAllowenceDetail = new GetBasicAllowDetailViewModel()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = 10,
                    PageNo = 1,
                },
                RevenuesTypeEnum = RevenuesTypeEnum.Sale
            };

            var basicAllowanceDetail = wrapper.GetBasicAllowanceDetails(requestByBasicAllowenceDetail);

            if (basicAllowanceDetail.ResponseMessage.ErrorCode == 0)
            {
                var list = basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
                {
                    AllowanceStateID = adr.AllowanceStateID,
                    AllowanceStateName = adr.AllowanceStateName,
                    Price = adr.Price
                });

                return View("AllowedDetails", list);
            }

            return View();
        }


        public ActionResult SetupAllowedDetails()
        {
            var wrapper = new WebServiceWrapper();

            var requestByBasicAllowenceDetail = new GetBasicAllowDetailViewModel()
            {
                PageInfo = new PageSettingsByWebService()
                {
                    ItemPerPage = 10,
                    PageNo = 1,
                },
                RevenuesTypeEnum = RevenuesTypeEnum.Setup
            };

            var basicAllowanceDetail = wrapper.GetBasicAllowanceDetails(requestByBasicAllowenceDetail);

            if (basicAllowanceDetail.ResponseMessage.ErrorCode == 0)
            {
                var list = basicAllowanceDetail.AllowanceDetailsResponse.Select(adr => new GetBasicAllowenceDetailsResponseList()
                {
                    AllowanceStateID = adr.AllowanceStateID,
                    AllowanceStateName = adr.AllowanceStateName,
                    Price = adr.Price
                });

                return View("AllowedDetails", list);
            }

            return View();
        }
    }
}