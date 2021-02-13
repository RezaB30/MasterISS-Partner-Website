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

                            if (userValid != null || (userSignInModel.DealerCode == userSignInModel.Username))
                            {
                                var claims = new List<Claim>
                                    {
                                        new Claim("UserMail", userSignInModel.DealerCode),
                                        new Claim("UserPassword", userSignInModel.Password),
                                        new Claim("PartnerName", authenticateResponse.AuthenticationResponse.DisplayName),
                                        new Claim("PartnerId", authenticateResponse.AuthenticationResponse.UserID.ToString()),
                                    };

                                if (authenticateResponse.AuthenticationResponse.Permissions.Select(pl => pl.Name).Contains("Setup"))
                                {
                                    claims.Add(new Claim("SetupServiceHash", authenticateResponse.AuthenticationResponse.SetupServiceHash));
                                    claims.Add(new Claim("SetupServiceUser", authenticateResponse.AuthenticationResponse.SetupServiceUser));
                                }

                                for (int i = 0; i < authenticateResponse.AuthenticationResponse.Permissions.Count(); i++)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, authenticateResponse.AuthenticationResponse.Permissions.Select(p => p.Name).ToArray()[i]));
                                    claims.Add(new Claim("RoleId", authenticateResponse.AuthenticationResponse.Permissions.Select(p => p.ID).ToArray()[i].ToString()));
                                }

                                if (userSignInModel.Username == userSignInModel.DealerCode)//Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                    claims.Add(new Claim(ClaimTypes.NameIdentifier, authenticateResponse.AuthenticationResponse.UserID.ToString()));
                                    claims.Add(new Claim(ClaimTypes.Email, userSignInModel.Username));
                                    claims.Add(new Claim(ClaimTypes.Name, authenticateResponse.AuthenticationResponse.DisplayName));
                                    if (Request.GetOwinContext().AdminSignIn(claims))
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                                else//SubUser
                                {
                                    var claimRoleList = claims.Where(c => c.Type == "RoleId").Select(c => c.Value);

                                    List<int> partnerRoleList = new List<int>();

                                    foreach (var item in claimRoleList)
                                    {
                                        partnerRoleList.Add(Convert.ToInt32(item));
                                    }

                                    var currentPermissionIdListByPartnerList = new List<int>();

                                    foreach (var roleId in partnerRoleList)
                                    {
                                        var permissionIdListByPartnerRoleList = db.Permission.Where(p => p.RoleTypeId == roleId).Select(p => p.Id).ToArray();

                                        foreach (var permissionId in permissionIdListByPartnerRoleList)
                                        {
                                            currentPermissionIdListByPartnerList.Add(permissionId);
                                        }
                                    }

                                    //claims.AddRange(userValid.Role.RolePermission.Select(r => new Claim(ClaimTypes.Role, r.Permission.PermissionName));

                                    foreach (var permissionId in currentPermissionIdListByPartnerList)
                                    {
                                        var userAvaibleRoles = userValid.Role.RolePermission.Where(rp => rp.PermissionId == permissionId).Select(p => p.Permission).ToArray();

                                        for (int i = 0; i < userAvaibleRoles.Length; i++)
                                        {
                                            claims.Add(new Claim(ClaimTypes.Role, userAvaibleRoles.Select(uar => uar.PermissionName).ToArray()[i]));
                                        }
                                    }

                                    var authenticator = new SubUserAuthenticator();
                                    var isSignIn = authenticator.SignIn(Request.GetOwinContext(), userSignInModel.Username, userSignInModel.Password, claims);

                                    if (isSignIn)
                                    {
                                        return RedirectToAction("Index", "Home");
                                    }
                                }
                            }
                            ViewBag.AuthenticateError = "Başarısız";
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
            var authenticator = new SubUserAuthenticator();
            authenticator.SignOut(Request.GetOwinContext());
            return RedirectToAction("SignIn", "Account");
        }
    }
}