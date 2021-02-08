using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(UserSignInViewModel userSignInModel)
        {
            if (ModelState.IsValid)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSignInModel.Username),
                    new Claim("UserMail", userSignInModel.DealerCode),
                    new Claim("UserPassword", userSignInModel.Password)
                };

                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(identity);
                return RedirectToAction("Index","Bill");
            }
            return View(userSignInModel);
        }

        public ActionResult Signout()
        {
            //var authenticator = new PartnerAuthencation();
            //authenticator.SignOut(Request.GetOwinContext());
            return RedirectToAction("SignIn", "Account");
        }
    }
}