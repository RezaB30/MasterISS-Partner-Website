﻿using System.Web;
using System.Web.Optimization;

namespace MasterISS_Partner_WebSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/custom-css").IncludeDirectory("~/Content/css/", "*.css"));


            bundles.Add(new ScriptBundle("~/QueryAdress").Include(
                "~/Scripts/QueryAddress/address-query.js"));

            bundles.Add(new ScriptBundle("~/CustomMap").Include(
               "~/Scripts/Map/map.js"));

            bundles.Add(new ScriptBundle("~/bundleMask/jquery").Include("~/Scripts/jquery.mask.js", "~/Scripts/input-upparcase.js"));

        }
    }
}
