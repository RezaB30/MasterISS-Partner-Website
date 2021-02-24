using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Helpers
{
    public static class TimeFormatHelper
    {
        public static MvcHtmlString TimeCustomFormat<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model;
            var date = (TimeSpan)value;

            TagBuilder wrapper = new TagBuilder("div");
            //td.SetInnerText(date.Hours + Localisation.View.Hours + date.Minutes + Localisation.View.Minutes + date.Seconds + Localisation.View.Seconds);

            if (date.Days != 0)
            {
                TagBuilder valueSpan = new TagBuilder("span");
                valueSpan.SetInnerText(date.Days.ToString());
                wrapper.InnerHtml += valueSpan.ToString(TagRenderMode.Normal);
                TagBuilder nameSpan = new TagBuilder("span");
                nameSpan.SetInnerText(Localization.View.Day);
                nameSpan.AddCssClass("time-span-parameter-name");
                wrapper.InnerHtml += nameSpan.ToString(TagRenderMode.Normal);
            }
            if (date.Hours != 0)
            {
                TagBuilder valueSpan = new TagBuilder("span");
                valueSpan.SetInnerText(date.Hours.ToString());
                wrapper.InnerHtml += valueSpan.ToString(TagRenderMode.Normal);
                TagBuilder nameSpan = new TagBuilder("span");
                nameSpan.SetInnerText(Localization.View.Hours);
                nameSpan.AddCssClass("time-span-parameter-name");
                wrapper.InnerHtml += nameSpan.ToString(TagRenderMode.Normal);
            }
            if (date.Minutes != 0)
            {
                TagBuilder valueSpan = new TagBuilder("span");
                valueSpan.SetInnerText(date.Minutes.ToString());
                wrapper.InnerHtml += valueSpan.ToString(TagRenderMode.Normal);
                TagBuilder nameSpan = new TagBuilder("span");
                nameSpan.SetInnerText(Localization.View.Minute);
                nameSpan.AddCssClass("time-span-parameter-name");
                wrapper.InnerHtml += nameSpan.ToString(TagRenderMode.Normal);
            }
            if (date.Seconds != 0)
            {
                TagBuilder valueSpan = new TagBuilder("span");
                valueSpan.SetInnerText(date.Seconds.ToString());
                wrapper.InnerHtml += valueSpan.ToString(TagRenderMode.Normal);
                TagBuilder nameSpan = new TagBuilder("span");
                nameSpan.SetInnerText(Localization.View.Second);
                nameSpan.AddCssClass("time-span-parameter-name");
                wrapper.InnerHtml += nameSpan.ToString(TagRenderMode.Normal);
            }
            //td.SetInnerText((date.Days == 0 ? "" : Localisation.View.Days + date.Hours == 0 ? "" : Localisation.View.Hours + date.Minutes == 0 ? "" : Localisation.View.Minutes + date.Seconds == 0 ? "" : Localisation.View.Seconds);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));

        }
    }
}