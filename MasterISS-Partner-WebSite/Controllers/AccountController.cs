using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.Authentication;
using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

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

                                if (authenticateResponse.AuthenticationResponse.Permissions.Select(pl => pl.Name).Contains(PartnerTypeEnum.Setup.ToString()))
                                {
                                    var validSetupInfo = db.PartnerSetupInfo.Find(authenticateResponse.AuthenticationResponse.UserID);
                                    if (validSetupInfo == null)
                                    {
                                        PartnerSetupInfo partnerSetupInfo = new PartnerSetupInfo
                                        {
                                            PartnerId=authenticateResponse.AuthenticationResponse.UserID,
                                            SetupServiceHash=authenticateResponse.AuthenticationResponse.SetupServiceHash,
                                            SetupServiceUser=authenticateResponse.AuthenticationResponse.SetupServiceUser
                                        };
                                        db.PartnerSetupInfo.Add(partnerSetupInfo);
                                        db.SaveChanges();
                                    }
                                    claims.Add(new Claim("SetupServiceHash", authenticateResponse.AuthenticationResponse.SetupServiceHash));
                                    claims.Add(new Claim("SetupServiceUser", authenticateResponse.AuthenticationResponse.SetupServiceUser));
                                }

                                var partnerPermissionList = authenticateResponse.AuthenticationResponse.Permissions.Select(ar => new
                                {
                                    claimRoleNames = new Claim(ClaimTypes.Role, ar.Name),
                                    claimRoleIds = new Claim("RoleId", ar.ID.ToString())
                                }).ToArray();

                                claims.AddRange(partnerPermissionList.Select(a => a.claimRoleNames));
                                claims.AddRange(partnerPermissionList.Select(a => a.claimRoleIds));

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
                                    var subUserPermission = userValid.Role.RolePermission.Select(m => new Claim(ClaimTypes.Role, m.Permission.PermissionName)).ToList();
                                    claims.AddRange(subUserPermission);



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

                    //LOG
                    LoggerError.Fatal($"An error occurred while Authenticate , ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage}  by: {userSignInModel.Username}");
                    //LOG

                    ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                    return View(userSignInModel);
                }
                //LOG
                LoggerError.Fatal($"An error occurred while Authenticate , ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage} by: {userSignInModel.Username}");
                //LOG

                ViewBag.AuthenticateError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(authenticateResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
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