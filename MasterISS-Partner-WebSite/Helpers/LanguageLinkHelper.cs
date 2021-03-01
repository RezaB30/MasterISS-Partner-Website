using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MasterISS_Partner_WebSite.Helpers
{
    public static class LanguageLinkHelper
    {
        public static MvcHtmlString LanguageLink(this HtmlHelper helper, string linkText, string culture)
        {
            var currentRouteData = helper.ViewContext.RouteData.Values;
            HttpContext.Current.Request.QueryString.CopyTo(currentRouteData);
            object action;
            currentRouteData.TryGetValue("action", out action);
            currentRouteData["sender"] = action;
            currentRouteData["culture"] = culture;

            return helper.ActionLink(linkText, "Language", currentRouteData);
        }
    }
}