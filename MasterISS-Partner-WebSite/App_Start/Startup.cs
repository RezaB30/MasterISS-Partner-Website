using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Globalization;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(MasterISS_Partner_WebSite.App_Start.Startup))]

namespace MasterISS_Partner_WebSite.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = "MasterISSPartnerWebsite",
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(value: "/Account/SignIn"),//yetkisi olmayan
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest
            });
        }
    }
}
