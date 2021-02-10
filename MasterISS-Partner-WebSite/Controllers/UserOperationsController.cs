using MasterISS_Partner_WebSite.Models;
using MasterISS_Partner_WebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Partner_WebSite.Controllers
{
    public class UserOperationsController : Controller
    {
        // GET: UserOperations
        public ActionResult Index()
        {
            var wrapper = new WebServiceWrapper();
            var partnerId = Convert.ToInt32(wrapper.PartnerId());

            using (var db = new PartnerWebSiteEntities())
            {
                var adminRoleId = db.Role.Where(r => r.RoleName == "Admin").FirstOrDefault().Id;

                var userList = db.User.Where(u => u.PartnerId == partnerId && u.RoleId != adminRoleId).Select(u => new
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
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(NewUserViewModel newUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var wrapper = new WebServiceWrapper();

                var response = wrapper.AddUser(newUserViewModel);
                if (ValidEmail(newUserViewModel.UserEmail))
                {
                    using (var db = new PartnerWebSiteEntities())
                    {
                        int? subuserRoleId = db.Role.Where(r => r.RoleName == "SubUser").SingleOrDefault().Id;

                        if (response.ResponseMessage.ErrorCode == 0)
                        {
                            if (subuserRoleId.HasValue)
                            {
                                User newUser = new User()
                                {
                                    IsEnabled = true,
                                    PartnerId = Convert.ToInt32(wrapper.PartnerId()),
                                    Password = wrapper.CalculateHash<SHA256>(newUserViewModel.Password),
                                    RoleId = (int)subuserRoleId,
                                    UserSubMail = newUserViewModel.UserEmail,
                                    NameSurname = newUserViewModel.UserNameSurname
                                };
                                db.User.Add(newUser);
                                db.SaveChanges();

                                return RedirectToAction("Successful");
                            }
                            ViewBag.AddUserError = Localization.View.ErrorProcessing;
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