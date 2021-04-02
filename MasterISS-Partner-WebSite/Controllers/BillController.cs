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
using MasterISS_Partner_WebSite_Enums;

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

       
    }
}