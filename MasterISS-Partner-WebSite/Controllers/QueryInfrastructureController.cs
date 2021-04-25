using MasterISS_Partner_WebSite.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize]
    public class QueryInfrastructureController : BaseController
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public ActionResult Index()
        {
            var wrapper = new WebServiceWrapper();
            var provinceList = wrapper.GetProvince();

            if (!string.IsNullOrEmpty(provinceList.ErrorMessage))
            {
                ViewBag.ErrorMessage = provinceList.ErrorMessage;
            }

            ViewBag.Provinces = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult DistrictList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var districtList = wrapper.GetDistricts(id);
            var list = districtList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = districtList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RuralRegionsList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var ruralRegionsList = wrapper.GetRuralRegions(id);
            var list = ruralRegionsList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = ruralRegionsList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NeighborhoodList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var neighborhoodList = wrapper.GetNeigbourhood(id);
            var list = neighborhoodList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = neighborhoodList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StreetList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var streetList = wrapper.GetStreets(id);
            var list = streetList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = streetList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BuildingList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var buildList = wrapper.GetBuildings(id);
            var list = buildList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = buildList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApartmentList(long id)
        {
            var wrapper = new WebServiceWrapper();
            var apartmentList = wrapper.GetApartments(id);
            var list = apartmentList.Data.ValueNamePairList.Select(data => new { Name = data.Name, Value = data.Value }).ToArray();

            return Json(new { list = list, errorMessage = apartmentList.ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceAvailability(string bbk)
        {
            var wrapper = new WebServiceWrapper();

            var response = wrapper.ServiceAvailability(bbk);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                //LOG
                wrapper = new WebServiceWrapper();
                LoggerError.Fatal("An error occurred while ServiceAvailability , ErrorMessage: " + response.ErrorMessage + ", by: " + wrapper.GetUserSubMail());
                //LOG
                TempData["Error"] = response.ErrorMessage;
                return RedirectToAction("Index");
            }


            var serviceAvaibilityViewModel = new ServiceAvaibilityResponseViewModel()
            {
                FIBER = new FIBER()
                {
                    HasInfrastructureFiber = response.Data.ServiceAvailabilityResponse.FIBER.HasInfrastructureFiber,
                    FiberDistance = response.Data.ServiceAvailabilityResponse.FIBER.FiberDistance ?? 0,
                    FiberVUID = response.Data.ServiceAvailabilityResponse.FIBER.FiberSVUID,
                    FiberPortState = response.Data.ServiceAvailabilityResponse.FIBER.FiberPortState,
                    FiberSpeed = response.Data.ServiceAvailabilityResponse.FIBER.FiberSpeed / 1024 ?? 0,
                    PortState = response.Data.ServiceAvailabilityResponse.FIBER.PortState
                },
                ADSL = new ADSL()
                {
                    AdslPortState = response.Data.ServiceAvailabilityResponse.ADSL.AdslPortState,
                    AdslDistance = response.Data.ServiceAvailabilityResponse.ADSL.AdslDistance ?? 0,
                    AdslSpeed = response.Data.ServiceAvailabilityResponse.ADSL.AdslSpeed / 1024 ?? 0,
                    AdslVUID = response.Data.ServiceAvailabilityResponse.ADSL.AdslSVUID,
                    HasInfrastructureAdsl = response.Data.ServiceAvailabilityResponse.ADSL.HasInfrastructureAdsl,
                    PortState = response.Data.ServiceAvailabilityResponse.ADSL.PortState
                },
                VDSL = new VDSL()
                {
                    HasInfrastructureVdsl = response.Data.ServiceAvailabilityResponse.VDSL.HasInfrastructureVdsl,
                    VdslDistance = response.Data.ServiceAvailabilityResponse.VDSL.VdslDistance??0,
                    VdslPortState = response.Data.ServiceAvailabilityResponse.VDSL.VdslPortState,
                    VdslSpeed = response.Data.ServiceAvailabilityResponse.VDSL.VdslSpeed / 1024 ?? 0,
                    VdslVUID = response.Data.ServiceAvailabilityResponse.VDSL.VdslSVUID,
                    PortState = response.Data.ServiceAvailabilityResponse.VDSL.PortState
                }
            };
            return View(serviceAvaibilityViewModel);
        }

    }
}