using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Helpers
{
    public static class CustomMap
    {
        public static MvcHtmlString Map(this HtmlHelper helper, string containerDiv, string mapName, string searchedPlaceInputPlaceholderName)
        {
            /*HEADER*/
            TagBuilder headerTitle = new TagBuilder("h3");
            headerTitle.SetInnerText(mapName);

            TagBuilder titleTD = new TagBuilder("td");
            titleTD.InnerHtml += headerTitle.ToString(TagRenderMode.Normal);

            TagBuilder closeButton = new TagBuilder("button");
            closeButton.Attributes.Add("type", "button");
            closeButton.Attributes.Add("style", "float:right");
            closeButton.AddCssClass("close-button");
            closeButton.SetInnerText("X");

            TagBuilder buttonTD = new TagBuilder("td");
            buttonTD.InnerHtml += closeButton.ToString(TagRenderMode.Normal);

            TagBuilder headerTR = new TagBuilder("tr");
            headerTR.InnerHtml += titleTD.ToString(TagRenderMode.Normal);
            headerTR.InnerHtml += buttonTD.ToString(TagRenderMode.Normal);

            TagBuilder headerTable = new TagBuilder("table");
            headerTable.Attributes.Add("style", "width:100%");
            headerTable.InnerHtml += headerTR.ToString(TagRenderMode.Normal);

            TagBuilder headerModal = new TagBuilder("div");
            headerModal.AddCssClass("m-header");
            headerModal.InnerHtml += headerTable.ToString(TagRenderMode.Normal);
            /*HEADER*/

            /*BODY*/
            TagBuilder searchedPlace = new TagBuilder("input");
            searchedPlace.Attributes.Add("placeholder", searchedPlaceInputPlaceholderName);
            searchedPlace.Attributes.Add("type", "text");
            searchedPlace.GenerateId("searced-place-input");

            TagBuilder formModal = new TagBuilder("form");
            formModal.Attributes.Add("style", "display:none");
            formModal.InnerHtml += searchedPlace.ToString(TagRenderMode.Normal);

            TagBuilder bodySpan = new TagBuilder("span");
            bodySpan.Attributes.Add("type", "text");
            bodySpan.GenerateId("selected-place-label");

            TagBuilder googleMap = new TagBuilder("div");
            googleMap.GenerateId("map_locations");


            TagBuilder bodyModal = new TagBuilder("div");
            bodyModal.AddCssClass("m-body");
            bodyModal.InnerHtml += formModal.ToString(TagRenderMode.Normal);
            bodyModal.InnerHtml += bodySpan.ToString(TagRenderMode.Normal);
            bodyModal.InnerHtml += googleMap.ToString(TagRenderMode.Normal);
            //body

            TagBuilder contentModal = new TagBuilder("div");
            contentModal.AddCssClass("m-content");
            contentModal.InnerHtml += headerModal.ToString(TagRenderMode.Normal);
            contentModal.InnerHtml += bodyModal.ToString(TagRenderMode.Normal);

            TagBuilder content = new TagBuilder("div");
            content.AddCssClass("content-wrap");
            content.InnerHtml += contentModal.ToString(TagRenderMode.Normal);

            TagBuilder container = new TagBuilder("div");
            container.AddCssClass(containerDiv);
            container.Attributes.Add("style", "display:none");
            container.InnerHtml += content.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(container.ToString());
        }
    }
}