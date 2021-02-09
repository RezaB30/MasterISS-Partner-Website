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
                var wrapper = new WebServiceWrapper();

                var authenticateResponse = wrapper.Authenticate(userSignInModel);

                if (authenticateResponse.ResponseMessage.ErrorCode == 0)
                {
                    if (authenticateResponse.AuthenticationResponse.IsAuthenticated == true)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userSignInModel.Username),//Submail
                            new Claim("UserMail", userSignInModel.DealerCode),
                            new Claim("UserPassword", userSignInModel.Password),
                            new Claim("PartnerName", authenticateResponse.AuthenticationResponse.DisplayName),
                            new Claim("UserId", authenticateResponse.AuthenticationResponse.UserID.ToString()),
                            new Claim("SetupServiceHash", authenticateResponse.AuthenticationResponse.SetupServiceHash),
                            new Claim("SetupServiceUser", authenticateResponse.AuthenticationResponse.SetupServiceUser),
                        };
                        for (int i = 0; i < authenticateResponse.AuthenticationResponse.Permissions.Count(); i++)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, authenticateResponse.AuthenticationResponse.Permissions.Select(p=>p.Name).ToArray()[i]));
                        }

                        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                        HttpContext.GetOwinContext().Authentication.SignIn(identity);
                        return RedirectToAction("Index", "Home");

                    }
                    ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                    return View(userSignInModel);
                }
                ViewBag.AuthenticateError = authenticateResponse.ResponseMessage.ErrorMessage;
                return View(userSignInModel);
            }
            return View(userSignInModel);
        }

        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("SignIn", "Account");
        }
    }
}