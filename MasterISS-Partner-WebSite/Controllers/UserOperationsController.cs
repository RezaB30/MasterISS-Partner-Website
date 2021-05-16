using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite_Business.Concrete;
using MasterISS_Partner_WebSite_DataAccess.Concrete.EntityFramework;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
using NLog;
using PagedList;
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
        public ActionResult Index(FilterUserViewModel filterUserViewModel, int page = 1, int pageSize = 12)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = Convert.ToInt32(claimInfo.PartnerId());
            var adminId = claimInfo.UserId();
            filterUserViewModel = filterUserViewModel ?? new FilterUserViewModel();
            ViewBag.Search = filterUserViewModel;

            //UserManager userManager = new UserManager(new EfUserDal());

            using (var db = new PartnerWebSiteEntities())
            {
                var userList = FilteredUserList(filterUserViewModel);

                var list = userList.Select(u => new UserListViewModel
                {
                    IsEnabled = u.IsEnabled,
                    NameSurname = u.NameSurname,
                    UserSubMail = u.UserSubMail,
                    RoleName = db.Role.Find(u.RoleId)?.RoleName,
                    UserId = u.Id,
                    PhoneNumber = u.PhoneNumber,
                    ısSetupTeam = db.SetupTeam.Where(st => st.UserId == u.Id && st.WorkingStatus == true).FirstOrDefault() != null,
                    SetupTeamUserAddressInfo = db.WorkArea.Where(wa => wa.UserId == u.Id).FirstOrDefault() == null ? Enumerable.Empty<SetupTeamUserAddressInfo>() : db.WorkArea.Where(wa => wa.UserId == u.Id).Select(wa => new SetupTeamUserAddressInfo
                    {
                        ProvinceName = wa.ProvinceName,
                        DistrictName = wa.Districtname,
                        RuralName = wa.RuralName,
                        NeigborhoodName = wa.NeighbourhoodName,
                    }).ToList()
                }).OrderBy(u => u.IsEnabled == true).ThenBy(u => u.UserId).Reverse().ToList();

                ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");
                ViewBag.FilterList = PermissionListByFilter(filterUserViewModel.SelectedPermission ?? null);

                var totalCount = list.Count();

                var pagedListByResponseList = new StaticPagedList<UserListViewModel>(list.Skip((page - 1) * pageSize).Take(pageSize), page, pageSize, totalCount);


                return View(pagedListByResponseList);
            }
        }

        private List<User> FilteredUserList(FilterUserViewModel filter)
        {
            var userList = Enumerable.Empty<User>().AsQueryable();

            var claimInfo = new ClaimInfo();
            var partnerId = Convert.ToInt32(claimInfo.PartnerId());
            var adminId = claimInfo.UserId();
            using (var db = new PartnerWebSiteEntities())
            {
                userList = db.User.Where(u => u.PartnerId == partnerId && u.Id != adminId);

                if (!string.IsNullOrEmpty(filter.SelectedUsername))
                {
                    var list = userList.Where(ul => ul.NameSurname.Contains(filter.SelectedUsername));
                    userList = list;
                }

                if (filter.SelectedPermission != null)
                {
                    var list = userList.SelectMany(ul => ul.Role.RolePermission.Where(rp => rp.PermissionId == filter.SelectedPermission), (ul, rp) => new { UserList = ul }).Select(filteredUserList => filteredUserList.UserList);
                    userList = list;
                }
                return userList.ToList();
            }
        }


        [Authorize(Roles = "Setup")]
        public ActionResult SetupTeamList()
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimList = new ClaimInfo();
                var partnerId = claimList.PartnerId();

                var setupTeamList = db.SetupTeam.Where(st => st.IsAdmin == false && st.User.PartnerId == partnerId && st.User.Role.RolePermission.Select(rp => rp.Permission.Id).Contains((int)PermissionListEnum.SetupManager)).Select(st => new SetupTeamListViewModel
                {
                    Id = st.UserId,
                    UserDisplayName = st.User.NameSurname,
                    WorkingStatus = st.WorkingStatus,
                    SetupTeamUserAddressInfo = st.User.WorkArea.Select(wa => new SetupTeamUserAddressInfo
                    {
                        ProvinceName = wa.ProvinceName,
                        DistrictName = wa.Districtname,
                        RuralName = wa.RuralName,
                        NeigborhoodName = wa.NeighbourhoodName,
                    })
                }).ToList();

                return View(setupTeamList);
            }
        }

        public ActionResult DeleteStaffWorkArea(long workAreaId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var wrapper = new WebServiceWrapper();

                var workArea = db.WorkArea.Find(workAreaId);
                if (workArea != null)
                {
                    db.WorkArea.Remove(workArea);
                    db.SaveChanges();

                    //LOG
                    Logger.Info("Deleted Work Area Id: " + workAreaId + ", by: " + wrapper.GetUserSubMail());
                    //LOG
                    return Json(new { status = "Success", message = Localization.View.Successful }, JsonRequestBehavior.AllowGet); ;
                }
                else
                {
                    //LOG
                    LoggerError.Error("Not Found Work AreaID in WorkAreaTable: " + workAreaId + ", by: " + wrapper.GetUserSubMail());
                    //LOG
                    return Json(new { status = "Failed", ErrorMessage = Localization.View.Generic200ErrorCodeMessage }, JsonRequestBehavior.AllowGet); ;
                }
            }
        }

        [Authorize(Roles = "Setup")]
        public ActionResult RendezvousTeamList()
        {
            var claimList = new ClaimInfo();
            var partnerId = claimList.PartnerId();
            using (var db = new PartnerWebSiteEntities())
            {
                var partnerTeam = db.RendezvousTeam.Where(rt => rt.IsAdmin == false && rt.User.PartnerId == partnerId && rt.User.Role.RolePermission.Select(rp => rp.Permission.Id).Contains((int)PermissionListEnum.RendezvousTeam)).Select(rt => new ListRendezvousTeamViewModel
                {
                    Id = rt.UserId,
                    NameSurname = rt.User.NameSurname,
                    WorkingStatus = rt.WorkingStatus
                }).ToList();

                return View(partnerTeam);
            }
        }

        public ActionResult AddPermission()
        {
            ViewBag.PermissionList = PermissionList();
            return PartialView("_AddPermission");
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

                    var validRolename = db.Role.Where(r => r.RoleName == addPermissionViewModel.RoleName && r.PartnerId == partnerId && r.IsEnabled).FirstOrDefault();
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
                                    PartnerId = partnerId,
                                    IsEnabled = true,
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
                                var message = Localization.View.Successful;
                                return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                            }
                            var errorMessage = Localization.View.Generic200ErrorCodeMessage;
                            return Json(new { status = "FailedAndRedirect", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                        }
                        var roleValidPermission = Localization.View.RoleValidPermission;
                        return Json(new { status = "Failed", ErrorMessage = roleValidPermission }, JsonRequestBehavior.AllowGet);
                    }
                    var avaibleRole = Localization.View.AvaibleRole;
                    return Json(new { status = "Failed", ErrorMessage = avaibleRole }, JsonRequestBehavior.AllowGet);
                }
            }
            var errorMessageModelState = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessageModelState }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUser()
        {
            var db = new PartnerWebSiteEntities();

            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();
            ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

            return PartialView("_AddUser");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(NewUserViewModel newUserViewModel)
        {
            var claimInfo = new ClaimInfo();
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
                                    RoleId = newUserViewModel.RoleId,
                                    PhoneNumber = newUserViewModel.PhoneNumber
                                };
                                db.User.Add(newUser);
                                db.SaveChanges();

                                if (ValidRoleHaveSetupManagerPermission(newUserViewModel.RoleId))
                                {
                                    SetupTeam setupTeam = new SetupTeam
                                    {
                                        UserId = newUser.Id,
                                        WorkingStatus = true,
                                        IsAdmin = false
                                    };
                                    db.SetupTeam.Add(setupTeam);
                                    db.SaveChanges();
                                }

                                if (ValidRoleHaveRendezvousTeamPermission(newUserViewModel.RoleId))
                                {
                                    RendezvousTeam rendezvousTeam = new RendezvousTeam
                                    {
                                        UserId = newUser.Id,
                                        WorkingStatus = true,
                                        IsAdmin = false
                                    };
                                    db.RendezvousTeam.Add(rendezvousTeam);
                                    db.SaveChanges();
                                }

                                //LOG
                                wrapper = new WebServiceWrapper();
                                Logger.Info("Added User: " + newUser.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                                //LOG

                                return Json(new { status = "Success", message = Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                            }
                            //LOG
                            var wrapperGetUserSubMail = new WebServiceWrapper();
                            LoggerError.Fatal($"An error occurred while AddUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage: {response.ResponseMessage.ErrorMessage}, by: {wrapperGetUserSubMail.GetUserSubMail()}");
                            //LOG

                            var webServiceResponse = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                            return Json(new { status = "FailedAndRedirect", ErrorMessage = webServiceResponse }, JsonRequestBehavior.AllowGet);
                        }
                        var notDefinedRole = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefinedRole }, JsonRequestBehavior.AllowGet);
                    }
                }
                var avaibleMailError = Localization.View.MailValidError;
                return Json(new { status = "Failed", ErrorMessage = avaibleMailError }, JsonRequestBehavior.AllowGet);
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet); ;
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
                    ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

                    var userViewModel = new UpdateUserRoleViewModel()
                    {
                        RoleId = (int)userAvaibleRole,
                        UserEmail = user.UserSubMail,
                        UserNameSurname = user.NameSurname,
                        UserId = userId,
                        PhoneNumber = user.PhoneNumber
                    };

                    return PartialView("_UpdateUserRole", userViewModel);
                }
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                return Content(contect);
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
                    var validRole = db.Role.Where(r => r.Id == updateUserRoleViewModel.RoleId && r.IsEnabled).FirstOrDefault();
                    if (validRole != null)
                    {
                        var user = db.User.Find(updateUserRoleViewModel.UserId);
                        if (user != null)
                        {
                            var wrapper = new WebServiceWrapper();

                            user.RoleId = updateUserRoleViewModel.RoleId;
                            user.PhoneNumber = updateUserRoleViewModel.PhoneNumber;
                            if (!string.IsNullOrEmpty(updateUserRoleViewModel.Password))
                            {
                                user.Password = wrapper.CalculateHash<SHA256>(updateUserRoleViewModel.Password);
                            }
                            db.SaveChanges();

                            if (!ValidRoleHaveSetupManagerPermission(updateUserRoleViewModel.RoleId))
                            {
                                var haveSetupManagerPermission = db.SetupTeam.Find(user.Id);
                                if (haveSetupManagerPermission != null)
                                {
                                    if (user.SetupTeam.WorkingStatus == true)
                                    {
                                        user.SetupTeam.WorkingStatus = false;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var haveSetupManagerPermission = db.SetupTeam.Find(user.Id);
                                if (haveSetupManagerPermission == null)
                                {
                                    SetupTeam setupTeam = new SetupTeam
                                    {
                                        UserId = user.Id,
                                        WorkingStatus = true,
                                        IsAdmin = false
                                    };
                                    db.SetupTeam.Add(setupTeam);
                                }
                                else
                                {
                                    if (haveSetupManagerPermission.WorkingStatus == false)
                                    {
                                        haveSetupManagerPermission.WorkingStatus = true;
                                    }
                                }
                                db.SaveChanges();
                            }

                            if (!ValidRoleHaveRendezvousTeamPermission(updateUserRoleViewModel.RoleId))
                            {
                                var haveRendezvousTeamPermission = db.RendezvousTeam.Find(user.Id);
                                if (haveRendezvousTeamPermission != null)
                                {
                                    if (user.RendezvousTeam.WorkingStatus == true)
                                    {
                                        user.RendezvousTeam.WorkingStatus = false;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                var haveRendezvousTeamPermission = db.RendezvousTeam.Find(user.Id);
                                if (haveRendezvousTeamPermission == null)
                                {
                                    RendezvousTeam rendezvousTeam = new RendezvousTeam
                                    {
                                        UserId = user.Id,
                                        WorkingStatus = true,
                                        IsAdmin = false
                                    };
                                    db.RendezvousTeam.Add(rendezvousTeam);
                                }
                                else
                                {
                                    if (haveRendezvousTeamPermission.WorkingStatus == false)
                                    {
                                        haveRendezvousTeamPermission.WorkingStatus = true;
                                    }
                                }
                                db.SaveChanges();
                            }

                            //LOG
                            wrapper = new WebServiceWrapper();
                            Logger.Info("Updated User Role: " + user.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                            //LOG

                            return Json(new { status = "Success", message = Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                        }
                        var contectNotFoundUser = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                        return Content(contectNotFoundUser);
                    }
                    var contectNotFoundRole = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                    return Content(contectNotFoundRole);
                }
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoleList()
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            using (var db = new PartnerWebSiteEntities())
            {
                var currentRoleList = db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new AvailableRoleList
                {
                    RoleId = r.Id,
                    RoleName = r.RoleName
                }).ToList();

                return View(currentRoleList);
            }
        }

        public ActionResult SetupTeamWorkingDaysAndHours(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(userId);
                if (user != null)
                {
                    var setupTeamStaffsToMatchedTheTask = new SetupTeamWorkingDaysAndHoursViewModel
                    {
                        AvailableWorkingDays = UserWorkingDays(user.UserId),
                        WorkingEndTime = $"{user.WorkEndTime:hh\\:mm}",
                        WorkingStartTime = $"{user.WorkStartTime:hh\\:mm}",
                        UserId = userId,
                        ContectName = user.User.NameSurname
                    };

                    return PartialView("_SetupTeamWorkingDaysAndHours", setupTeamStaffsToMatchedTheTask);
                }
                else
                {
                    var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                    return Content(contect);
                }
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SetupTeamWorkingDaysAndHours(SetupTeamWorkingDaysAndHoursViewModel setupTeamWorkingDaysAndHoursView)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in setupTeamWorkingDaysAndHoursView.SelectedDays)
                {
                    if (!Enum.IsDefined(typeof(MasterISS_Partner_WebSite_Enums.Enums.DayOfWeekEnum), item))
                    {
                        var notDefined = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }

                using (var db = new PartnerWebSiteEntities())
                {
                    var user = db.SetupTeam.Find(setupTeamWorkingDaysAndHoursView.UserId);
                    if (user != null)
                    {
                        user.WorkStartTime = ParseTimeSpan(setupTeamWorkingDaysAndHoursView.WorkingStartTime);
                        user.WorkEndTime = ParseTimeSpan(setupTeamWorkingDaysAndHoursView.WorkingEndTime);
                        user.WorkDays = string.Join(",", setupTeamWorkingDaysAndHoursView.SelectedDays);
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
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        private List<AvailableWorkingDays> UserWorkingDays(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(userId);

                var localizedList = new LocalizedList<MasterISS_Partner_WebSite_Enums.Enums.DayOfWeekEnum, Localization.DayList>();

                var userCurrentWorkDays = user.WorkDays?.Split(',');

                var userAvailableWorkingDays = localizedList.GenericList.Select(gl => new AvailableWorkingDays
                {
                    DayId = gl.ID,
                    DayName = gl.Name,
                    IsSelected = userCurrentWorkDays == null ? false : userCurrentWorkDays.Contains(gl.ID.ToString()),
                }).ToList();

                foreach (var item in userAvailableWorkingDays)
                {
                    item.DayName = localizedList.GetDisplayText(item.DayId, CultureInfo.CurrentCulture);
                }

                return userAvailableWorkingDays;
            }
        }
        private TimeSpan ParseTimeSpan(string parsedValue)
        {
            var timeSpanValue = DateTime.ParseExact(parsedValue, "HH:mm", null).TimeOfDay;
            return timeSpanValue;
        }


        public ActionResult UpdateRolePermission()
        {
            using (var db = new PartnerWebSiteEntities())
            {
                return PartialView("_UpdateRolePermission");
            }
        }

        public ActionResult RolePermissionList(int roleId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var claimInfo = new ClaimInfo();
                var rolePermissionList = db.RolePermission.Where(rp => rp.RoleId == roleId).Select(p => p.PermissionId);
                if (rolePermissionList != null)
                {
                    var adminRoleIdList = claimInfo.PartnerRoleId().ToArray();
                    var localizedList = new LocalizedList<PermissionListEnum, Localization.PermissionList>();

                    var permissionList = db.Permission.Where(p => adminRoleIdList.Contains(p.RoleTypeId)).Select(p => new AvailablePermissionList()
                    {
                        PermissionId = p.Id,
                        PermissionName = p.PermissionName,
                        IsSelected = rolePermissionList.Contains(p.Id) == true ? "checked" : null,
                    }).ToArray();

                    foreach (var item in permissionList)
                    {
                        item.PermissionName = localizedList.GetDisplayText(item.PermissionId, CultureInfo.CurrentCulture);
                    }
                    return Json(new { status = "Success", list = permissionList }, JsonRequestBehavior.AllowGet);
                }
                var notDefined = Localization.View.Generic200ErrorCodeMessage;
                return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateRolePermission(AvailablePermissionList availablePermissionList, int? roleId)
        {
            if (roleId != null)
            {
                if (ModelState.IsValid)
                {
                    var claimList = new ClaimInfo();
                    var partnerId = claimList.PartnerId();
                    using (var db = new PartnerWebSiteEntities())
                    {
                        var validRolename = db.Role.Where(r => r.Id == roleId && r.PartnerId == partnerId && r.IsEnabled).FirstOrDefault();
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
                                    RoleId = roleId.Value
                                });
                                db.RolePermission.AddRange(addedRolePermission);
                                db.SaveChanges();


                                var currentUser = db.User.Where(u => u.RoleId == roleId && u.PartnerId == partnerId && u.IsEnabled).Select(u => u.Id).ToList();

                                if (currentUser.Count > 0)
                                {
                                    if (!ValidRoleHaveSetupManagerPermission(roleId.Value))
                                    {
                                        var matchedSetupTeams = db.SetupTeam.Where(st => currentUser.Contains(st.UserId));

                                        if (matchedSetupTeams.Count() > 0)
                                        {
                                            foreach (var item in matchedSetupTeams)
                                            {
                                                item.WorkingStatus = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var currentSetupTeam = db.SetupTeam.Select(st => st.UserId);
                                        var newSetupTeam = currentUser.Where(u => currentSetupTeam.Contains(u) == false).Select(st => new SetupTeam
                                        {
                                            UserId = st,
                                            WorkingStatus = true
                                        });
                                        db.SetupTeam.AddRange(newSetupTeam);

                                        var matchedSetupTeams = db.SetupTeam.Where(st => currentUser.Contains(st.UserId));
                                        if (matchedSetupTeams.Count() > 0)
                                        {
                                            foreach (var item in matchedSetupTeams)
                                            {
                                                item.WorkingStatus = true;
                                            }
                                        }
                                    }

                                    if (!ValidRoleHaveRendezvousTeamPermission(roleId.Value))
                                    {
                                        var matchedRendezvousTeams = db.RendezvousTeam.Where(rt => currentUser.Contains(rt.UserId));
                                        if (matchedRendezvousTeams.Count() > 0)
                                        {
                                            foreach (var item in matchedRendezvousTeams)
                                            {
                                                item.WorkingStatus = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var currentRendezvousTeam = db.RendezvousTeam.Select(rt => rt.UserId);
                                        var newRendezvousTeam = currentUser.Where(u => currentRendezvousTeam.Contains(u) == false).Select(rt => new RendezvousTeam
                                        {
                                            UserId = rt,
                                            WorkingStatus = true
                                        });

                                        db.RendezvousTeam.AddRange(newRendezvousTeam);

                                        var matchedRendezvousTeam = db.RendezvousTeam.Where(rt => currentUser.Contains(rt.UserId));
                                        if (matchedRendezvousTeam.Count() > 0)
                                        {
                                            foreach (var item in matchedRendezvousTeam)
                                            {
                                                item.WorkingStatus = true;
                                            }
                                        }

                                    }
                                    db.SaveChanges();
                                }
                                return Json(new { status = "Success", message = Localization.View.Successful }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        var notDefined = Localization.View.Generic200ErrorCodeMessage;
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }
                var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
            var notfoundRoleId = Localization.View.RequiredRoleId;
            return Json(new { status = "Failed", ErrorMessage = notfoundRoleId }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Setup")]
        public ActionResult AddAndUpdateWorkAreaSetupTeamUser(int userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(userId);
                if (user != null && user.WorkingStatus == true)
                {
                    var wrapper = new WebServiceWrapper();
                    var provinceList = wrapper.GetProvince();
                    if (!string.IsNullOrEmpty(provinceList.ErrorMessage))
                    {
                        var contectErrorByWebService = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                        return Content(contectErrorByWebService);
                    }
                    ViewBag.Provinces = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                    ViewBag.District = new SelectList("");
                    ViewBag.Rurals = new SelectList("");
                    ViewBag.Neigborhoods = new SelectList("");

                    var workAreaSetupTeamModel = new WorkAreaSetupTeamUserViewModel
                    {
                        ContactName = user.User.NameSurname,
                        SetupTeamUserAddressInfo = user.User.WorkArea.Select(wa => new SetupTeamUserAddressInfo
                        {
                            ProvinceName = wa.ProvinceName,
                            DistrictName = wa.Districtname,
                            RuralName = wa.RuralName,
                            NeigborhoodName = wa.NeighbourhoodName,
                            Id = wa.Id
                        }),
                        UserId = userId,
                    };
                    return PartialView("_AddAndUpdateWorkAreaSetupTeamUser", workAreaSetupTeamModel);
                }
                var contect = string.Format("<script language='javascript' type='text/javascript'>GetAlert('{0}','false','{1}');</script>", new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture), Url.Action("Index", "Home"));
                return Content(contect);
            }

        }

        [Authorize(Roles = "Setup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAndUpdateWorkAreaSetupTeamUser(WorkAreaSetupTeamUserViewModel workAreaSetupTeamUserViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    var user = db.SetupTeam.Find(workAreaSetupTeamUserViewModel.UserId);
                    if (user != null && user.WorkingStatus == true)
                    {
                        if (ValidWorkArea(workAreaSetupTeamUserViewModel))
                        {
                            WorkArea workArea = new WorkArea
                            {
                                UserId = workAreaSetupTeamUserViewModel.UserId,
                                ProvinceId = workAreaSetupTeamUserViewModel.ProvinceId,
                                ProvinceName = workAreaSetupTeamUserViewModel.ProvinceName,
                                DistrictId = workAreaSetupTeamUserViewModel.DistrictId ?? null,
                                Districtname = workAreaSetupTeamUserViewModel.DistrictName,
                                NeighbourhoodId = workAreaSetupTeamUserViewModel.NeigborhoodId ?? null,
                                NeighbourhoodName = workAreaSetupTeamUserViewModel.NeigborhoodName,
                                RuralId = workAreaSetupTeamUserViewModel.RuralId ?? null,
                                RuralName = workAreaSetupTeamUserViewModel.RuralName
                            };
                            db.WorkArea.Add(workArea);
                            db.SaveChanges();

                            //LOG
                            var wrapper = new WebServiceWrapper();
                            Logger.Info($"Added New WorkArea User: {user.User.UserSubMail}, by: {wrapper.GetUserSubMail()}");
                            //LOG

                            return Json(new { status = "Success" }, JsonRequestBehavior.AllowGet); ;
                        }
                        var validWorkArea = Localization.View.ValidWorkArea;
                        return Json(new { status = "Failed", ErrorMessage = validWorkArea }, JsonRequestBehavior.AllowGet); ;
                    }
                    var notDefinedUserOrWorkingArea = Localization.View.Generic200ErrorCodeMessage;
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefinedUserOrWorkingArea }, JsonRequestBehavior.AllowGet);
                }
            }
            var errorMessage = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { status = "Failed", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet); ;
        }

        private bool ValidWorkArea(WorkAreaSetupTeamUserViewModel workAreaSetupTeamUserViewModel)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(workAreaSetupTeamUserViewModel.UserId);
                var userWorkAreas = user.User.WorkArea.Select(wa => new { provinceId = wa.ProvinceId, districtId = wa.DistrictId, ruralId = wa.RuralId, neigborhoodId = wa.NeighbourhoodId });


                foreach (var userWorkArea in userWorkAreas)
                {
                    if (userWorkArea.provinceId == workAreaSetupTeamUserViewModel.ProvinceId && userWorkArea.districtId.HasValue == false)
                    {
                        return false;
                    }
                    if (workAreaSetupTeamUserViewModel.DistrictId.HasValue && userWorkArea.districtId == workAreaSetupTeamUserViewModel.DistrictId && userWorkArea.ruralId.HasValue == false)
                    {
                        return false;
                    }

                    if (workAreaSetupTeamUserViewModel.RuralId.HasValue && userWorkArea.ruralId == workAreaSetupTeamUserViewModel.RuralId && userWorkArea.neigborhoodId.HasValue == false)
                    {
                        return false;
                    }

                    if (workAreaSetupTeamUserViewModel.NeigborhoodId.HasValue && userWorkArea.neigborhoodId == workAreaSetupTeamUserViewModel.NeigborhoodId)
                    {
                        return false;
                    }
                }

                if (workAreaSetupTeamUserViewModel.DistrictId.HasValue == false)
                {
                    var userSelectedProvinceWorkArea = db.WorkArea.Where(wa => wa.UserId == workAreaSetupTeamUserViewModel.UserId && wa.ProvinceId == workAreaSetupTeamUserViewModel.ProvinceId).ToList();
                    if (userSelectedProvinceWorkArea.Count > 0)
                    {
                        db.WorkArea.RemoveRange(userSelectedProvinceWorkArea);
                        db.SaveChanges();
                    }
                }
                if (workAreaSetupTeamUserViewModel.RuralId.HasValue == false)
                {
                    var userSelectedProvinceWorkArea = db.WorkArea.Where(wa => wa.UserId == workAreaSetupTeamUserViewModel.UserId && wa.DistrictId == workAreaSetupTeamUserViewModel.DistrictId && wa.DistrictId.HasValue).ToList();
                    if (userSelectedProvinceWorkArea.Count > 0)
                    {
                        db.WorkArea.RemoveRange(userSelectedProvinceWorkArea);
                        db.SaveChanges();
                    }
                }
                if (workAreaSetupTeamUserViewModel.NeigborhoodId.HasValue == false)
                {
                    var userSelectedProvinceWorkArea = db.WorkArea.Where(wa => wa.UserId == workAreaSetupTeamUserViewModel.UserId && wa.NeighbourhoodId == workAreaSetupTeamUserViewModel.NeigborhoodId && wa.NeighbourhoodId.HasValue).ToList();
                    if (userSelectedProvinceWorkArea.Count > 0)
                    {
                        db.WorkArea.RemoveRange(userSelectedProvinceWorkArea);
                        db.SaveChanges();
                    }
                }

                return true;
            }
        }

        [Authorize(Roles = "Setup")]
        public ActionResult ListWorkAreaSetupTeamUser(int userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(userId);
                if (user != null && user.WorkingStatus == true)
                {
                    var userWorkArea = user.User.WorkArea.Select(wa => new ListWorkAreaSetupTeamUserViewModel
                    {
                        WorkAreaId = wa.Id,
                        SetupTeamUserAddressInfo = new SetupTeamUserAddressInfo
                        {
                            DistrictName = wa.Districtname,
                            ProvinceName = wa.ProvinceName,
                            RuralName = wa.RuralName,
                            NeigborhoodName = wa.NeighbourhoodName
                        }
                    });
                    ViewBag.UserNameSurname = user.User.NameSurname;
                    return View(userWorkArea);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EnableUser(long userId)
        {
            var wrapper = new WebServiceWrapper();
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.User.Find(userId);
                if (user != null)
                {
                    var response = wrapper.EnableUser(user.UserSubMail);

                    if (response.ResponseMessage.ErrorCode == 0)
                    {
                        user.IsEnabled = true;

                        var userIsSetupTeam = db.SetupTeam.Find(userId);
                        if (userIsSetupTeam != null)
                        {
                            userIsSetupTeam.WorkingStatus = true;
                        }

                        var userIsRandezvousTeam = db.RendezvousTeam.Find(userId);
                        if (userIsRandezvousTeam != null)
                        {
                            userIsRandezvousTeam.WorkingStatus = true;
                        }
                        //LOG
                        wrapper = new WebServiceWrapper();
                        Logger.Info("Enabled User: " + user.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        db.SaveChanges();
                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //LOG
                        wrapper = new WebServiceWrapper();
                        LoggerError.Fatal($"An error occurred while EnableUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapper.GetUserSubMail()}");
                        //LOG
                        var notDefined = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var notDefined = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture);
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult DisableUser(long userId)
        {
            var wrapper = new WebServiceWrapper();
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.User.Find(userId);
                if (user != null)
                {
                    var response = wrapper.DisableUser(user.UserSubMail);

                    if (response.ResponseMessage.ErrorCode == 0)
                    {
                        user.IsEnabled = false;

                        var userIsSetupTeam = db.SetupTeam.Find(userId);
                        if (userIsSetupTeam != null)
                        {
                            userIsSetupTeam.WorkingStatus = false;
                        }

                        var userIsRandezvousTeam = db.RendezvousTeam.Find(userId);
                        if (userIsRandezvousTeam != null)
                        {
                            userIsRandezvousTeam.WorkingStatus = false;
                        }
                        //LOG
                        wrapper = new WebServiceWrapper();
                        Logger.Info("Disabled User: " + user.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG
                        db.SaveChanges();

                        var message = Localization.View.Successful;
                        return Json(new { status = "Success", message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var wrapperGetUserSubMail = new WebServiceWrapper();
                        LoggerError.Fatal($"An error occurred while DisableUser , ErrorCode: {response.ResponseMessage.ErrorCode}, ErrorMessage : {response.ResponseMessage.ErrorMessage} by: {wrapperGetUserSubMail.GetUserSubMail()}");
                        //LOG
                        var notDefined = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText(response.ResponseMessage.ErrorCode, CultureInfo.CurrentCulture);
                        return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var notDefined = new LocalizedList<ErrorCodesEnum, Localization.ErrorCodesList>().GetDisplayText((int)ErrorCodesEnum.Failed, CultureInfo.CurrentCulture);
                    return Json(new { status = "FailedAndRedirect", ErrorMessage = notDefined }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Successful()
        {
            return View();
        }


        private SelectList PermissionList()
        {
            var permissionList = LocalizedPermissionList();
            var selectListsPermission = new SelectList(permissionList.Select(pl => new { Value = pl.PermissionId, Text = pl.PermissionName }), "Value", "Text");
            return selectListsPermission;
        }

        private AvailablePermissionList[] LocalizedPermissionList()
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
                item.PermissionName = localizedList.GetDisplayText(item.PermissionId, CultureInfo.CurrentCulture);
            }
            return permissionList;
        }

        private SelectList PermissionListByFilter(int? selectedValue)
        {
            var permissionList = LocalizedPermissionList();

            int[] filterList = new int[]
            {
                (int)PermissionListEnum.SetupManager,
                (int)PermissionListEnum.RendezvousTeam,
                (int)PermissionListEnum.SaleManager,
                (int)PermissionListEnum.PaymentManager
            };

            var selectListsPermission = new SelectList(permissionList.Where(pl => filterList.Contains(pl.PermissionId)).Select(pl => new { Value = pl.PermissionId, Text = pl.PermissionName }), "Value", "Text", selectedValue);
            return selectListsPermission;
        }

        private bool ValidRoleHaveSetupManagerPermission(int roleId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var role = db.Role.Find(roleId);
                var roleHaveSetupManagerPermission = role.RolePermission.Select(rp => rp.Permission.Id).Contains((int)PermissionListEnum.SetupManager);
                if (roleHaveSetupManagerPermission)
                {
                    return true;
                }
                return false;
            }
        }

        private bool ValidRoleHaveRendezvousTeamPermission(int roleId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var role = db.Role.Find(roleId);
                var roleHaveSetupManagerPermission = role.RolePermission.Select(rp => rp.Permission.Id).Contains((int)PermissionListEnum.RendezvousTeam);
                if (roleHaveSetupManagerPermission)
                {
                    return true;
                }
                return false;
            }
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
    }
}