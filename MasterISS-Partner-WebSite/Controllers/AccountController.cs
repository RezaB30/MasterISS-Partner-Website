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

                            if (userValid != null || (userSignInModel.DealerCode == userSignInModel.Username))
                            {
                                var claims = new List<Claim>
                                    {
                                        new Claim("UserMail", userSignInModel.DealerCode),
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

                                if (userSignInModel.Username == userSignInModel.DealerCode)//Admin
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                    claims.Add(new Claim(ClaimTypes.NameIdentifier, authenticateResponse.AuthenticationResponse.UserID.ToString()));
                                    claims.Add(new Claim(ClaimTypes.Email, userSignInModel.Username));
                                    claims.Add(new Claim(ClaimTypes.Name, authenticateResponse.AuthenticationResponse.DisplayName));

                                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                                    Request.GetOwinContext().Authentication.SignIn(identity);
                                }
                                else//SubUser
                                {
                                    //var currentPermissionIdListByPartnerList = new List<int>();

                                    //foreach (var roleId in authenticateResponse.AuthenticationResponse.Permissions.ToArray())
                                    //{
                                    //    //var permissionIdListByPartnerRoleList = db.Permission.Where(p => p.RoleTypeId ==  roleId.ID).Select(p => p.Id).ToArray();


                                    //    foreach (var permissionId in permissionIdListByPartnerRoleList)
                                    //    {
                                    //        currentPermissionIdListByPartnerList.Add(permissionId);
                                    //    }
                                    //}
                                    var permissionIdListByPartnerRoleList = db.Permission.Where(p => authenticateResponse.AuthenticationResponse.Permissions.Select(s => s.ID).Contains((short)p.RoleTypeId))
                                        .Select(p => p.Id).ToArray();
                                    claims.AddRange(userValid.Role.RolePermission.Where(r => permissionIdListByPartnerRoleList.Contains(r.PermissionId))
                                        .Select(r => new Claim(ClaimTypes.Role, r.Permission.PermissionName)));

                                    //foreach (var permissionId in currentPermissionIdListByPartnerList)
                                    //{
                                    //    var userAvaibleRoles = userValid.Role.RolePermission.Where(rp => rp.PermissionId == permissionId).Select(p => p.Permission).ToArray();

                                    //    for (int i = 0; i < userAvaibleRoles.Length; i++)
                                    //    {
                                    //        claims.Add(new Claim(ClaimTypes.Role, userAvaibleRoles.Select(uar => uar.PermissionName).ToArray()[i]));
                                    //    }
                                    //}

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