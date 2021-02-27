﻿using MasterISS_Partner_WebSite.Enums;
using MasterISS_Partner_WebSite.Models;
using MasterISS_Partner_WebSite.ViewModels;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserOperationsController : BaseController
    {
        // GET: UserOperations
        public ActionResult Index()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = Convert.ToInt32(claimInfo.PartnerId());

            using (var db = new PartnerWebSiteEntities())
            {
                var userList = db.User.Where(u => u.PartnerId == partnerId).Select(u => new
                {
                    IsEnabled = u.IsEnabled,
                    NameSurname = u.NameSurname,
                    UserSubMail = u.UserSubMail,
                    UserRole = u.Role.RoleName,
                    UserId = u.Id
                }).ToList();

                var userlistViewModel = new List<UserListViewModel>();
                foreach (var item in userList)
                {
                    userlistViewModel.Add(new UserListViewModel()
                    {
                        IsEnabled = item.IsEnabled,
                        NameSurname = item.NameSurname,
                        UserSubMail = item.UserSubMail,
                        RoleName = item.UserRole,
                        UserId = item.UserId
                    });
                }
                return View(userlistViewModel);
            }
        }

        public ActionResult AddPermission()
        {
            var claimInfo = new ClaimInfo();
            var adminRoleIdList = claimInfo.PartnerRoleId().ToArray();
            var db = new PartnerWebSiteEntities();
            var list = new List<SelectListItem>();
            var localizedList = new LocalizedList<PermissionListEnum, Localization.PermissionList>();
            var permissionList = db.Permission.Where(p => adminRoleIdList.Contains(p.RoleTypeId)).Select(p => new SelectListItem()
            {
                Value = p.Id.ToString()
            }).ToArray();

            foreach (var item in permissionList)
            {
                item.Text = localizedList.GetDisplayText(Convert.ToInt32(item.Value), null);
            }

            var permissionModel = new AddPermissionViewModel()
            {
                AvaibleRole = permissionList
            };
            return View(permissionModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddPermission(AddPermissionViewModel addPermissionViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var claimList = new ClaimInfo();
                    var partnerId = claimList.PartnerId();

                    var validRolename = db.Role.Where(r => r.RoleName == addPermissionViewModel.RoleName && r.PartnerId == partnerId).FirstOrDefault();
                    if (validRolename == null)
                    {
                        if (addPermissionViewModel.SelectedRole != null && addPermissionViewModel.SelectedRole.Count > 0)
                        {
                            var role = new Role()
                            {
                                RoleName = addPermissionViewModel.RoleName,
                                PartnerId = partnerId
                            };
                            db.Role.Add(role);

                            for (int i = 0; i < addPermissionViewModel.SelectedRole.Count; i++)
                            {
                                var permissionId = addPermissionViewModel.SelectedRole.ToArray()[i];
                                var convertedRoleId = Convert.ToInt32(permissionId);

                                var rolePermission = new RolePermission()
                                {
                                    RoleId = role.Id,
                                    PermissionId = convertedRoleId
                                };
                                db.RolePermission.Add(rolePermission);
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index");
                        }
                        TempData["RoleValid"] = Localization.View.RoleValidPermission;
                        return RedirectToAction("AddPermission");
                    }
                    TempData["RoleValid"] = Localization.View.AvaibleRole;
                    return RedirectToAction("AddPermission");
                }
            }
            TempData["RoleValid"] = Localization.View.RoleNameValid;
            return Redirect("AddPermission");
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

                var response = wrapper.AddUser(newUserViewModel);
                if (ValidEmail(newUserViewModel.UserEmail))
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
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

                            return RedirectToAction("Successful");
                        }
                        else if (response.ResponseMessage.ErrorCode == 200)
                        {
                            ViewBag.AddUserError = Localization.View.Generic200ErrorCodeMessage;
                            return View(newUserViewModel);
                        }
                        ViewBag.AddUserError = response.ResponseMessage.ErrorMessage;
                        return View(newUserViewModel);
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
                        return RedirectToAction("Successful");
                    }
                    RedirectToAction("Index");

                }
            }
            return View(updateUserRoleViewModel);
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
                        return RedirectToAction("Successful");
                    }
                    TempData["Error"] = Localization.View.PassiveError;
                    return RedirectToAction("Index");
                }
            }
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                TempData["Error"] = Localization.View.Generic200ErrorCodeMessage;
                return RedirectToAction("Index");
            }
            TempData["Error"] = response.ResponseMessage.ErrorMessage;
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
                        return RedirectToAction("Successful");
                    }
                    TempData["Error"] = Localization.View.ActiveError;
                    return RedirectToAction("Index");
                }
            }
            else if (response.ResponseMessage.ErrorCode == 200)
            {
                TempData["Error"] = Localization.View.Generic200ErrorCodeMessage;
                return RedirectToAction("Index");
            }

            TempData["Error"] = response.ResponseMessage.ErrorMessage;
            return RedirectToAction("Index");
        }

        public ActionResult Successful()
        {
            return View();
        }

    }
}