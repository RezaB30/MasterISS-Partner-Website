using MasterISS_Partner_WebSite;
using MasterISS_Partner_WebSite.Authentication;
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
using MasterISS_Partner_WebSite_Enums;
using MasterISS_Partner_WebSite.ViewModels.Account;
using MasterISS_Partner_WebSite_WebServices.PartnerServiceReference;
using MasterISS_Partner_WebSite_Enums.Enums;

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

                            if (userValid != null)
                            {
                                var claims = new List<Claim>
                                    {
                                       new Claim("UserMail", userSignInModel.PartnerCode),
                                       new Claim(ClaimTypes.NameIdentifier, userValid.Id.ToString()),
                                       new Claim(ClaimTypes.Email, userValid.UserSubMail),
                                       new Claim(ClaimTypes.Name,userValid.NameSurname),
                                       new Claim(ClaimTypes.MobilePhone,userValid.PhoneNumber)
                                    };

                                ValidResponseHaveSetupPermissionAndAddClaims(authenticateResponse, claims);

                                var subUserPermission = userValid.Role.RolePermission.Select(m => new Claim(ClaimTypes.Role, m.Permission.PermissionName)).ToList();
                                claims.AddRange(subUserPermission);

                                var isSignIn = Authenticator.SignIn(Request.GetOwinContext(), claims);
                                if (isSignIn)
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                            return View(userSignInModel);
                        }
                    }

                    //LOG
                    LoggerError.Fatal($"An error occurred while UserAuthenticate , ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage}  by: {userSignInModel.Username}");
                    //LOG

                    ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                    return View(userSignInModel);
                }
                //LOG
                LoggerError.Fatal($"An error occurred while User Authenticate , ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage} by: {userSignInModel.Username}");
                //LOG

                ViewBag.AuthenticateError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(authenticateResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                return View(userSignInModel);
            }
            return View(userSignInModel);
        }

        public ActionResult AdminSignIn()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AdminSignIn(AdminSignInViewModel adminSignInModel)
        {
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();

                var authenticateResponse = wrapper.Authenticate(adminSignInModel);

                if (authenticateResponse.ResponseMessage.ErrorCode == 0)
                {
                    if (authenticateResponse.AuthenticationResponse.IsAuthenticated == true)
                    {
                        using (var db = new PartnerWebSiteEntities())
                        {
                            var adminPasswordHash = wrapper.CalculateHash<SHA256>(adminSignInModel.Password);
                            var adminValid = db.User.Where(u => u.Password == adminPasswordHash && u.UserSubMail == adminSignInModel.Username && u.RoleId == null && u.PartnerId == null).FirstOrDefault();
                            if (adminValid == null)
                            {
                                if (string.IsNullOrEmpty(authenticateResponse.AuthenticationResponse.PhoneNo))
                                {
                                    LoggerError.Fatal($"An error occurred while Admin Authenticate , ErrorMessage : PhoneNo is null. by: {adminSignInModel.Username}");
                                    ViewBag.AuthenticateError = Localization.View.Generic200ErrorCodeMessage;
                                    return View(adminSignInModel);
                                }

                                User user = new User
                                {
                                    IsEnabled = true,
                                    PartnerId = null,
                                    RoleId = null,
                                    UserSubMail = adminSignInModel.Username,
                                    Password = adminPasswordHash,
                                    NameSurname = authenticateResponse.AuthenticationResponse.DisplayName,
                                    PhoneNumber = authenticateResponse.AuthenticationResponse.PhoneNo
                                };
                                db.User.Add(user);
                                db.SaveChanges();
                                adminValid = db.User.Where(u => u.Password == adminPasswordHash && u.UserSubMail == adminSignInModel.Username && u.RoleId == null && u.PartnerId == null).FirstOrDefault();
                            }
                            var adminValidSetupTeamTable = db.SetupTeam.Find(adminValid.Id);
                            if (adminValidSetupTeamTable == null)
                            {
                                var localizedListWorkDays = new LocalizedList<DayOfWeekEnum, Localization.DayList>().GenericList.Select(l => l.ID.ToString());
                                var adminWorkDays = string.Join(",", localizedListWorkDays);
                                SetupTeam setupTeam = new SetupTeam
                                {
                                    IsAdmin = true,
                                    UserId = adminValid.Id,
                                    WorkingStatus = true,
                                    WorkDays = adminWorkDays,
                                    WorkStartTime = DateTime.ParseExact("01:00", "HH:mm", null).TimeOfDay,
                                    WorkEndTime = DateTime.ParseExact("23:59", "HH:mm", null).TimeOfDay,
                                };
                                db.SetupTeam.Add(setupTeam);
                                db.SaveChanges();
                            }

                            var adminValidRendezvousTeamTable = db.RendezvousTeam.Find(adminValid.Id);
                            if (adminValidRendezvousTeamTable == null)
                            {
                                RendezvousTeam rendezvousTeam = new RendezvousTeam
                                {
                                    IsAdmin = true,
                                    UserId = adminValid.Id,
                                    WorkingStatus = true,
                                };
                                db.RendezvousTeam.Add(rendezvousTeam);
                                db.SaveChanges();
                            }
                            

                            var claims = new List<Claim>
                            {
                                new Claim("UserMail", adminSignInModel.Username),
                                new Claim(ClaimTypes.Role, "Admin"),
                                new Claim(ClaimTypes.NameIdentifier, adminValid.Id.ToString()),
                                new Claim(ClaimTypes.Email, adminValid.UserSubMail),
                                new Claim(ClaimTypes.Name,adminValid.NameSurname),
                                new Claim(ClaimTypes.MobilePhone,adminValid.PhoneNumber),
                            };

                            ValidResponseHaveSetupPermissionAndAddClaims(authenticateResponse, claims);

                            var isSignIn = Authenticator.SignIn(Request.GetOwinContext(), claims);

                            if (isSignIn)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                    //LOG
                    LoggerError.Fatal($"An error occurred while Admin Authenticate , ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage}  by: {adminSignInModel.Username}");
                    //LOG

                    ViewBag.AuthenticateError = Localization.View.AuthenticateError;
                    return View(adminSignInModel);
                }
                //LOG
                LoggerError.Fatal($"An error occurred while Admin Authenticate, ErrorCode: {authenticateResponse.ResponseMessage.ErrorCode}, ErrorMessage : {authenticateResponse.ResponseMessage.ErrorMessage} by: {adminSignInModel.Username}");
                //LOG

                ViewBag.AuthenticateError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(authenticateResponse.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                return View(adminSignInModel);
            }

            return View(adminSignInModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UserSettings(UserSettingsViewModel userSettingsViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimInfo = new ClaimInfo();
                var userId = claimInfo.UserId();
                using (var db = new PartnerWebSiteEntities())
                {
                    var user = db.User.Find(userId);
                    if (user != null)
                    {
                        user.PhoneNumber = userSettingsViewModel.NumberPhone;
                        db.SaveChanges();

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var notDefined = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SignOut()
        {
            var authenticator = new SubUserAuthenticator();
            authenticator.SignOut(Request.GetOwinContext());
            return RedirectToAction("SignIn", "Account");
        }

        private void ValidResponseHaveSetupPermissionAndAddClaims(PartnerServiceAuthenticationResponse response, List<Claim> currentClaimList)
        {
            if (response.AuthenticationResponse.Permissions.Select(pl => pl.Name).Contains(PartnerTypeEnum.Setup.ToString()))
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var validSetupInfo = db.PartnerSetupInfo.Find(response.AuthenticationResponse.UserID);
                    if (validSetupInfo == null)
                    {
                        PartnerSetupInfo partnerSetupInfo = new PartnerSetupInfo
                        {
                            PartnerId = response.AuthenticationResponse.UserID,
                            SetupServiceHash = response.AuthenticationResponse.SetupServiceHash,
                            SetupServiceUser = response.AuthenticationResponse.SetupServiceUser
                        };
                        db.PartnerSetupInfo.Add(partnerSetupInfo);
                        db.SaveChanges();
                    }
                    else
                    {
                        validSetupInfo.SetupServiceHash = response.AuthenticationResponse.SetupServiceHash;
                        validSetupInfo.SetupServiceUser = response.AuthenticationResponse.SetupServiceUser;
                        db.SaveChanges();
                    }
                    currentClaimList.Add(new Claim("SetupServiceHash", response.AuthenticationResponse.SetupServiceHash));
                    currentClaimList.Add(new Claim("SetupServiceUser", response.AuthenticationResponse.SetupServiceUser));
                }
            }

            var partnerPermissionList = response.AuthenticationResponse.Permissions.Select(ar => new
            {
                claimRoleNames = new Claim(ClaimTypes.Role, ar.Name),
                claimRoleIds = new Claim("RoleId", ar.ID.ToString())
            }).ToArray();

            currentClaimList.AddRange(partnerPermissionList.Select(a => a.claimRoleNames));
            currentClaimList.AddRange(partnerPermissionList.Select(a => a.claimRoleIds));
            currentClaimList.Add(new Claim("PartnerName", response.AuthenticationResponse.DisplayName));
            currentClaimList.Add(new Claim("PartnerId", response.AuthenticationResponse.UserID.ToString()));




        }
    }
}