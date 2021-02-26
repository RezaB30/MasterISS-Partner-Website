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
    public class AccountController : BaseController
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
                            var userValid = db.User.Where(u => u.IsEnabled == true && u.PartnerId == authenticateResponse.AuthenticationResponse.UserID && u.Password == userPasswordHash && u.UserSubMail == userSignInModel.Username).FirstOrDefault();

                            if (userValid != null || (userSignInModel.PartnerCode == userSignInModel.Username))
                            {
                                var claims = new List<Claim>
                                    {
                                        new Claim("UserMail", userSignInModel.PartnerCode),
                                        new Claim("PartnerName", authenticateResponse.AuthenticationResponse.DisplayName),
                                        new Claim("PartnerId", authenticateResponse.AuthenticationResponse.UserID.ToString()),
                                    };

                                if (authenticateResponse.AuthenticationResponse.Permissions.Select(pl => pl.Name).Contains("Setup"))
                                {
                                    claims.Add(new Claim("SetupServiceHash", authenticateResponse.AuthenticationResponse.SetupServiceHash));
                                    claims.Add(new Claim("SetupServiceUser", authenticateResponse.AuthenticationResponse.SetupServiceUser));
                                }

                                foreach (var item in authenticateResponse.AuthenticationResponse.Permissions)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, item.Name));
                                    claims.Add(new Claim("RoleId", item.ID.ToString()));
                                }

                                if (userSignInModel.Username == userSignInModel.PartnerCode)//Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                    claims.Add(new Claim(ClaimTypes.NameIdentifier, authenticateResponse.AuthenticationResponse.UserID.ToString()));
                                    claims.Add(new Claim(ClaimTypes.Email, userSignInModel.Username));
                                    claims.Add(new Claim(ClaimTypes.Name, authenticateResponse.AuthenticationResponse.DisplayName));

                                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                                    Request.GetOwinContext().Authentication.SignIn(identity);

                                    return RedirectToAction("Index", "Home");
                                }
                                else//SubUser
                                {
                                    var responsePermissionId = authenticateResponse.AuthenticationResponse.Permissions.Select(arp => Convert.ToInt32(arp.ID));

                                    var avaiblePermissionName = db.Permission.SelectMany(permission => responsePermissionId.Where(r => r == permission.RoleTypeId), (permission, response) => new { permission.PermissionName }).Select(p => p.PermissionName);                                    
                                    foreach (var item in avaiblePermissionName)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, item));
                                    }

                                    var authenticator = new SubUserAuthenticator();
                                    var isSignIn = authenticator.SignIn(Request.GetOwinContext(), userSignInModel.Username, userSignInModel.Password, claims);

                                    if (isSignIn)
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                            }
                            ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                            return View(userSignInModel);
                        }
                    }
                    ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                    return View(userSignInModel);
                }
                else if (authenticateResponse.ResponseMessage.ErrorCode == 200)
                {
                    ViewBag.AuthenticateError = Localization.View.GeneralErrorDescription;
                    return View(userSignInModel);
                }
                ViewBag.AuthenticateError = authenticateResponse.ResponseMessage.ErrorMessage;
                return View(userSignInModel);
            }
            return View(userSignInModel);
        }

        public ActionResult SignOut()
        {
            var authenticator = new SubUserAuthenticator();
            authenticator.SignOut(Request.GetOwinContext());
            return RedirectToAction("SignIn", "Account");
        }
    }
}