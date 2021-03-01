using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class BaseController : Controller
    {
        private static Logger Logger = LogManager.GetLogger("AppLoggerError");

        // GET: Base
        protected override void OnException(ExceptionContext filterContext)
        {
            //Log
            var wrapper = new WebServiceWrapper();
            Logger.Fatal(filterContext.Exception, "User received error:{0}", wrapper.GetUserSubMail());
            //Log

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
            };
            filterContext.ExceptionHandled = true;

        }
    }
}