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
        PartnerWebSiteEntities db = new PartnerWebSiteEntities();
        // GET: UserOperations
        public ActionResult Index()
        {
            return View();
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

                int? subuserRoleId = db.Role.Where(r => r.RoleName == "SubUser").SingleOrDefault().Id;

                if (response.ResponseMessage.ErrorCode == 0)
                {
                    User newUser = new User()
                    {
                        IsEnabled = true,
                        PartnerId = Convert.ToInt32(wrapper.PartnerId()),
                        Password = wrapper.CalculateHash<SHA256>(newUserViewModel.Password),
                        RoleId = subuserRoleId ?? 2,
                        UserSubMail = newUserViewModel.UserEmail,
                        NameSurname = newUserViewModel.UserNameSurname
                    };
                    db.User.Add(newUser);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                ViewBag.AddUserError = response.ResponseMessage.ErrorMessage;
                return View(newUserViewModel);
            }
            return View(newUserViewModel);
        }

        public ActionResult EnableUser(string userMail)
        {
            var wrapper = new WebServiceWrapper();

            var response = wrapper.EnableUser(userMail);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var enabledUser = db.User.Where(m => m.UserSubMail == userMail).FirstOrDefault();
                if (enabledUser != null)
                {
                    enabledUser.IsEnabled = false;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Error = response.ResponseMessage.ErrorMessage;
            return View("Index");
        }

        public ActionResult DisableUser(string userMail)
        {

            var wrapper = new WebServiceWrapper();

            var response = wrapper.DisableUser(userMail);

            if (response.ResponseMessage.ErrorCode == 0)
            {
                var disabledUser = db.User.Where(m => m.UserSubMail == userMail).FirstOrDefault();
                if (disabledUser != null)
                {
                    disabledUser.IsEnabled = true;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Error = response.ResponseMessage.ErrorMessage;
            return View("Index");
        }

    }
}