﻿using MasterISS_Partner_WebSite.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MasterISS_Partner_WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            ModelBinders.Binders[typeof(DateTime?)] = new DateBinder();
            ModelBinders.Binders[typeof(DateTime)] = new DateBinder();

            //ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }
    }
}
