using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Helpers
{
    public static class CustomButtonByMap
    {
        public static MvcHtmlString MapTriggerButton(this HtmlHelper helper,string butttonId, string buttonText)
        {
            TagBuilder modalButton = new TagBuilder("input");
            modalButton.AddCssClass("select-button");
            modalButton.Attributes.Add("type", "button");
            modalButton.Attributes.Add("value", buttonText);
            modalButton.GenerateId(butttonId);
            return new MvcHtmlString(modalButton.ToString());
        }
    }
}