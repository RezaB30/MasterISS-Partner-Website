using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite.Models;
using MasterISS_Partner_WebSite.ViewModels;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserOperationsController : BaseController
    {
        private static Logger Logger = LogManager.GetLogger("AppLogger");
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        // GET: UserOperations
        public ActionResult Index()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = Convert.ToInt32(claimInfo.PartnerId());

            using (var db = new PartnerWebSiteEntities())
            {
                var userList = db.User.Where(u => u.PartnerId == partnerId).Select(u => new UserListViewModel
                {
                    IsEnabled = u.IsEnabled,
                    NameSurname = u.NameSurname,
                    UserSubMail = u.UserSubMail,
                    RoleName = u.Role.RoleName,
                    UserId = u.Id,
                }).ToList();

                return View(userList);
            }
        }

        public ActionResult AddPermission()
        {
            ViewBag.PermissionList = PermissionList();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddPermission(AddPermissionViewModel addPermissionViewModel)
        {
            ViewBag.PermissionList = PermissionList();

            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var claimList = new ClaimInfo();
                    var partnerId = claimList.PartnerId();

                    var validRolename = db.Role.Where(r => r.RoleName == addPermissionViewModel.RoleName && r.PartnerId == partnerId).FirstOrDefault();
                    if (validRolename == null)
                    {
                        if (addPermissionViewModel.AvailableRoles != null && addPermissionViewModel.AvailableRoles.Length > 0)
                        {
                            var partnerAvaibleRoles = claimList.PartnerRoleId();

                            var availablePartnerPermission = db.Permission.SelectMany(permission => partnerAvaibleRoles.Where(r => r == permission.RoleTypeId), (permission, response) => new { permission.Id }).Select(p => p.Id);

                            var userMatchedPermissionCount = addPermissionViewModel.AvailableRoles.Where(viewmodel => availablePartnerPermission.Contains(viewmodel) == true).Count();

                            if (userMatchedPermissionCount == addPermissionViewModel.AvailableRoles.Length)
                            {
                                var role = new Role()
                                {
                                    RoleName = addPermissionViewModel.RoleName,
                                    PartnerId = partnerId
                                };
                                db.Role.Add(role);

                                var rolePermissions = addPermissionViewModel.AvailableRoles.Select(ar => new RolePermission
                                {
                                    RoleId = role.Id,
                                    PermissionId = ar
                                });
                                db.RolePermission.AddRange(rolePermissions);

                                db.SaveChanges();

                                //LOG
                                var wrapper = new WebServiceWrapper();
                                Logger.Info("Added role: " + role.RoleName + ", by: " + wrapper.GetUserSubMail());
                                //LOG

                                return RedirectToAction("Index");
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        ViewBag.RoleValid = Localization.View.RoleValidPermission;
                        return View(addPermissionViewModel);
                    }
                    ViewBag.RoleValid = Localization.View.AvaibleRole;
                    return View(addPermissionViewModel);
                }
            }
            return View(addPermissionViewModel);
        }

        public ActionResult AddUser()
        {
            var db = new PartnerWebSiteEntities();

            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();
            ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(NewUserViewModel newUserViewModel)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            var dbRole = new PartnerWebSiteEntities();
            ViewBag.RoleList = new SelectList(dbRole.Role.Where(r => r.PartnerId == partnerId).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name", newUserViewModel.RoleId);

            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();

                if (ValidEmail(newUserViewModel.UserEmail))
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
                        var roleValid = db.Role.Find(newUserViewModel.RoleId);
                        if (roleValid != null)
                        {
                            var response = wrapper.AddUser(newUserViewModel);
                            if (response.ResponseMessage.ErrorCode == 0)
                            {
                                User newUser = new User()
                                {
                                    IsEnabled = true,
                                    PartnerId = Convert.ToInt32(claimInfo.PartnerId()),
                                    Password = wrapper.CalculateHash<SHA256>(newUserViewModel.Password),
                                    UserSubMail = newUserViewModel.UserEmail,
                                    NameSurname = newUserViewModel.UserNameSurname,
                                    RoleId = newUserViewModel.RoleId
                                };

                                db.User.Add(newUser);

                                db.SaveChanges();

                                //LOG
                                wrapper = new WebServiceWrapper();
                                Logger.Info("Added User: " + newUser.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                                //LOG

                                return RedirectToAction("Successful");
                            }
                            //LOG
                            var wrapperGetUserSubMail = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while AddUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {wrapperGetUserSubMail.GetUserSubMail()}");
                            //LOG

                            ViewBag.AddUserError = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                            return View(newUserViewModel);
                        }
                        return RedirectToAction("Index", "UserOperations");
                    }
                }
                ViewBag.AddUserError = Localization.View.MailValidError;
                return View(newUserViewModel);
            }
            return View(newUserViewModel);

        }

        public ActionResult UpdateUserRole(int userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var partnerId = claimInfo.PartnerId();
                var user = db.User.Find(userId);
                if (user != null)
                {
                    var userAvaibleRole = user.RoleId;
                    ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

                    var userViewModel = new UpdateUserRoleViewModel()
                    {
                        RoleId = userAvaibleRole,
                        UserEmail = user.UserSubMail,
                        UserNameSurname = user.NameSurname,
                        UserId = userId
                    };

                    return View(userViewModel);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateUserRole(UpdateUserRoleViewModel updateUserRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var validRole = db.Role.Where(r => r.Id == updateUserRoleViewModel.RoleId).FirstOrDefault();
                    if (validRole != null)
                    {
                        var user = db.User.Find(updateUserRoleViewModel.UserId);

                        user.RoleId = updateUserRoleViewModel.RoleId;
                        db.SaveChanges();

                        //LOG
                        var wrapper = new WebServiceWrapper();
                        Logger.Info("Updated User Role: " + user.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        return RedirectToAction("Successful");
                    }
                    RedirectToAction("Index");
                }
            }
            return View(updateUserRoleViewModel);
        }

        public ActionResult RoleList()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var currentRoleList = db.Role.Where(r => r.PartnerId == partnerId).Select(r => new AvailableRoleList
                {
                    RoleId = r.Id,
                    RoleName = r.RoleName
                }).ToList();

                return View(currentRoleList);
            }
        }

        public ActionResult UpdateRolePermission(int roleId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var rolePermissionList = db.RolePermission.Where(rp => rp.RoleId == roleId).Select(p => p.PermissionId);
                if (rolePermissionList != null)
                {
                    var claimInfo = new ClaimInfo();
                    var adminRoleIdList = claimInfo.PartnerRoleId().ToArray();
                    var localizedList = new LocalizedList<PermissionListEnum, Localization.PermissionList>();

                    var permissionList = db.Permission.Where(p => adminRoleIdList.Contains(p.RoleTypeId)).Select(p => new AvailablePermissionList()
                    {
                        PermissionId = p.Id,
                        PermissionName = p.PermissionName,
                        IsSelected = rolePermissionList.Contains(p.Id) == true,
                    }).ToArray();

                    foreach (var item in permissionList)
                    {
                        item.PermissionName = localizedList.GetDisplayText(item.PermissionId, null);
                    }

                    ViewBag.RoleId = roleId;
                    ViewBag.RoleName = db.Role.Find(roleId).RoleName;

                    return View(permissionList);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateRolePermission(AvailablePermissionList availablePermissionList, int roleId)
        {
            if (ModelState.IsValid)
            {
                var claimList = new ClaimInfo();
                var partnerId = claimList.PartnerId();
                using (var db = new PartnerWebSiteEntities())
                {
                    var validRolename = db.Role.Where(r => r.Id == roleId && r.PartnerId == partnerId).FirstOrDefault();
                    if (validRolename != null)
                    {
                        var partnerAvaibleRoles = claimList.PartnerRoleId();

                        var availablePartnerPermission = db.Permission.SelectMany(permission => partnerAvaibleRoles.Where(r => r == permission.RoleTypeId), (permission, response) => new { permission.Id }).Select(p => p.Id);

                        var userMatchedPermissionCount = availablePermissionList.SelectedPermissions.Where(viewmodel => availablePartnerPermission.Contains(viewmodel) == true).Count();

                        if (userMatchedPermissionCount == availablePermissionList.SelectedPermissions.Length)
                        {
                            var removedRolePermission = db.RolePermission.Where(rp => rp.RoleId == roleId).ToArray();
                            db.RolePermission.RemoveRange(removedRolePermission);
                            db.SaveChanges();

                            var addedRolePermission = availablePermissionList.SelectedPermissions.Select(selectedP => new RolePermission
                            {
                                PermissionId = selectedP,
                                RoleId = roleId
                            });
                            db.RolePermission.AddRange(addedRolePermission);
                            db.SaveChanges();
                        }
                        return RedirectToAction("Successful");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = Localization.View.RequiredPermission;
            return RedirectToAction("UpdateRolePermission", new { roleId = roleId });
        }


        private bool ValidEmail(string eMail)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var valid = db.User.Where(u => u.UserSubMail == eMail).FirstOrDefault();
                if (valid == null)
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult EnableUser(string userMail)
        {
            var wrapper = new WebServiceWrapper();

            var response = wrapper.EnableUser(userMail);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var enabledUser = db.User.Where(m => m.UserSubMail == userMail).FirstOrDefault();

                    if (enabledUser != null)
                    {
                        enabledUser.IsEnabled = true;
                        db.SaveChanges();

                        //LOG
                        wrapper = new WebServiceWrapper();
                        Logger.Info("Enabled User: " + userMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        return RedirectToAction("Successful");
                    }
                    TempData["Error"] = Localization.View.PassiveError;
                    return RedirectToAction("Index");
                }
            }
            //LOG
            wrapper = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while EnableUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
            //LOG
            TempData["Error"] = new LocalizedList<PermissionListEnum, Localization.PermissionList>().GetDisplayText(response.ResponseMessage.ErrorCode);
            return RedirectToAction("Index");
        }

        public ActionResult DisableUser(string userMail)
        {
            var wrapper = new WebServiceWrapper();

            var response = wrapper.DisableUser(userMail);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var disabledUser = db.User.Where(m => m.UserSubMail == userMail).FirstOrDefault();

                    if (disabledUser != null)
                    {
                        disabledUser.IsEnabled = false;
                        db.SaveChanges();

                        //LOG
                        wrapper = new WebServiceWrapper();
                        Logger.Info("Disabled User: " + userMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        return RedirectToAction("Successful");
                    }
                    TempData["Error"] = Localization.View.ActiveError;
                    return RedirectToAction("Index");
                }
            }
            //LOG
            var wrapperGetUserSubMail = new WebServiceWrapper();
            LoggerError.Fatal($"An error occurred while DisableUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperGetUserSubMail.GetUserSubMail()}");
            //LOG

            TempData["Error"] = new LocalizedList<PermissionListEnum, Localization.PermissionList>().GetDisplayText(response.ResponseMessage.ErrorCode);
            return RedirectToAction("Index");
        }

        public ActionResult Successful()
        {
            return View();
        }


        private SelectList PermissionList()
        {
            var claimInfo = new ClaimInfo();
            var adminRoleIdList = claimInfo.PartnerRoleId().ToArray();
            var db = new PartnerWebSiteEntities();
            var list = new List<SelectListItem>();
            var localizedList = new LocalizedList<PermissionListEnum, Localization.PermissionList>();
            var permissionList = db.Permission.Where(p => adminRoleIdList.Contains(p.RoleTypeId)).Select(p => new AvailablePermissionList()
            {
                PermissionId = p.Id,
                PermissionName = p.PermissionName
            }).ToArray();

            foreach (var item in permissionList)
            {
                item.PermissionName = localizedList.GetDisplayText(item.PermissionId, null);
            }
            var selectListsPermission = new SelectList(permissionList.Select(pl => new { Value = pl.PermissionId, Text = pl.PermissionName }), "Value", "Text");

            return selectListsPermission;
        }


    }
}