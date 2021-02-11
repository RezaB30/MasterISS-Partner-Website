using MasterISS_Partner_WebSite.Enums;
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
    public class UserOperationsController : Controller
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
                }).ToList();

                var userlistViewModel = new List<UserListViewModel>();
                foreach (var item in userList)
                {
                    userlistViewModel.Add(new UserListViewModel()
                    {
                        IsEnabled = item.IsEnabled,
                        NameSurname = item.NameSurname,
                        UserSubMail = item.UserSubMail,
                    });
                }
                return View(userlistViewModel);
            }
        }

        public ActionResult AddUser()
        {
            var claimInfo = new ClaimInfo();
            var adminRoleIdList = claimInfo.PartnerRoleId().ToArray();
            var db = new PartnerWebSiteEntities();
            var list = new List<SelectListItem>();
            var localizedList = new LocalizedList<PermissionListEnum, Localization.PermissionList>();

            for (int i = 0; i < adminRoleIdList.Length; i++)
            {
                var array = adminRoleIdList[i];
                var roleList = db.Role.Where(r => r.RoleTypeId.ToString() == array).Select(r => r.Id).ToArray();

                for (int j = 0; j < roleList.Length; j++)
                {
                    var listItem = new SelectListItem()
                    {
                        Value = roleList[j].ToString(),
                        Text = localizedList.GetDisplayText(roleList[j], null)
                    };
                    list.Add(listItem);
                }
            }

            var model = new NewUserViewModel()
            {
                AvaibleRole = list
            };
            return View(model);
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(NewUserViewModel newUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();
                var claimInfo = new ClaimInfo();

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
                                NameSurname = newUserViewModel.UserNameSurname
                            };

                            db.User.Add(newUser);

                            for (int i = 0; i < newUserViewModel.SelectedRole.Count; i++)
                            {
                                var roleId = newUserViewModel.SelectedRole.ToArray()[i];
                                var convertedRoleId = Convert.ToInt32(roleId);

                                var userRole = new UserRole()
                                {
                                    RoleId=convertedRoleId,
                                    UserId=newUser.Id
                                };
                                db.UserRole.Add(userRole);
                            }
                            db.SaveChanges();

                            return RedirectToAction("Successful");
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
            TempData["Error"] = response.ResponseMessage.ErrorMessage;
            return RedirectToAction("Index");
        }

        public ActionResult Successful()
        {
            return View();
        }

    }
}