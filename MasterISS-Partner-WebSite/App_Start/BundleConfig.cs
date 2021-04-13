using System.Web;
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

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.

            bundles.Add(new StyleBundle("~/Content/all-page-css").IncludeDirectory("~/Content/css/", "*.css"));


            bundles.Add(new ScriptBundle("~/QueryAdress").Include(
                "~/Scripts/QueryAddress/address-query.js"));

            bundles.Add(new ScriptBundle("~/CustomMap").Include(
               "~/Scripts/Map/map.js"));
            
            bundles.Add(new ScriptBundle("~/Alert").Include(
               "~/Scripts/Alert/sweet-alert.js"));

            bundles.Add(new ScriptBundle("~/Scripts/all-page-js").IncludeDirectory("~/Scripts/js/", "*.js"));

            bundles.Add(new ScriptBundle("~/bundleMask/jquery").Include("~/Scripts/jquery.mask.js", "~/Scripts/input-upparcase.js"));

        }
    }
}
