using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.Authentication;
using MasterISS_Partner_WebSite.Models;
using MasterISS_Partner_WebSite.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
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
                        using (var db = new PartnerWebSiteEntities())
                        {
                            var userPasswordHash = wrapper.CalculateHash<SHA256>(userSignInModel.Password);
                            var userValid = db.User.Where(u => u.IsEnabled == true && u.PartnerId == authenticateResponse.AuthenticationResponse.UserID && u.Password == userPasswordHash).FirstOrDefault();

                            if (userValid != null)
                            {
                                var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, userSignInModel.Username),
                                        new Claim("UserMail", userSignInModel.DealerCode),
                                        new Claim("UserPassword", userSignInModel.Password),
                                        new Claim("PartnerName", authenticateResponse.AuthenticationResponse.DisplayName),
                                        new Claim("UserId", authenticateResponse.AuthenticationResponse.UserID.ToString()),
                                        new Claim("SetupServiceHash", authenticateResponse.AuthenticationResponse.SetupServiceHash),
                                        new Claim("SetupServiceUser", authenticateResponse.AuthenticationResponse.SetupServiceUser),
                                    };
                                for (int i = 0; i < authenticateResponse.AuthenticationResponse.Permissions.Count(); i++)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, authenticateResponse.AuthenticationResponse.Permissions.Select(p => p.Name).ToArray()[i]));
                                    claims.Add(new Claim("RoleId", authenticateResponse.AuthenticationResponse.Permissions.Select(p => p.ID).ToArray()[i].ToString()));
                                }

                                if (userSignInModel.Username == userSignInModel.DealerCode)//Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                }

                                var authenticator = new PartnerAuthenticator();
                                var isSignIn = authenticator.SignIn(Request.GetOwinContext(), userSignInModel.Username, userSignInModel.Password, claims);

                                if (isSignIn)
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            ViewBag.AuthenticateError = "";
                            return View(userSignInModel);
                        }
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
            var authenticator = new PartnerAuthenticator();
            authenticator.SignOut(Request.GetOwinContext());
            return RedirectToAction("SignIn", "Account");
        }
    }
}