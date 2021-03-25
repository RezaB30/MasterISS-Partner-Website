using MasterISS_Partner_WebSite.ViewModels;
using MasterISS_Partner_WebSite_Database.Models;
using MasterISS_Partner_WebSite_Enums;
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
            var adminId = claimInfo.UserId();

            using (var db = new PartnerWebSiteEntities())
            {
                var userList = db.User.Where(u => u.PartnerId == partnerId && u.Id != adminId).Select(u => new UserListViewModel
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

                    return RedirectToAction("Successful");
                }
                else
                {
                    //LOG
                    LoggerError.Error("Not Found Work AreaID in WorkAreaTable: " + workAreaId + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    ViewBag.Error = Localization.View.Generic200ErrorCodeMessage;
                    return View("SetupTeamList");
                }
            }
        }

        [Authorize(Roles = "Setup")]
        public ActionResult EnabledUserInRendezvousTeam(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var rendezvousTeam = db.RendezvousTeam.Find(userId);

                if (rendezvousTeam != null)
                {
                    rendezvousTeam.WorkingStatus = true;

                    db.SaveChanges();

                    //LOG
                    var wrapper = new WebServiceWrapper();
                    Logger.Info("Enabled User In RendezvousTeam: " + userId + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    return RedirectToAction("Successful");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Setup")]
        public ActionResult DisabledUserInSetupTeam(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var setupTeam = db.SetupTeam.Find(userId);

                if (setupTeam != null)
                {
                    setupTeam.WorkingStatus = false;

                    db.SaveChanges();

                    //LOG
                    var wrapper = new WebServiceWrapper();
                    Logger.Info("Disabled User In SetupTeam: " + userId + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    return RedirectToAction("Successful");
                }
                return RedirectToAction("Index", "Home");
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

        [Authorize(Roles = "Setup")]
        public ActionResult DisabledUserInRendezvousTeam(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var rendezvousTeam = db.RendezvousTeam.Find(userId);

                if (rendezvousTeam != null)
                {
                    rendezvousTeam.WorkingStatus = false;

                    db.SaveChanges();

                    //LOG
                    var wrapper = new WebServiceWrapper();
                    Logger.Info("Disabled User In RendezvousTeam: " + userId + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    return RedirectToAction("Successful");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Setup")]
        public ActionResult EnabledUserInSetupTeam(long userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var setupTeam = db.SetupTeam.Find(userId);

                if (setupTeam != null)
                {
                    setupTeam.WorkingStatus = true;

                    db.SaveChanges();

                    //LOG
                    var wrapper = new WebServiceWrapper();
                    Logger.Info("Enabled User In SetupTeam: " + userId + ", by: " + wrapper.GetUserSubMail());
                    //LOG

                    return RedirectToAction("Successful");
                }
                return RedirectToAction("Index", "Home");
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
            ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddUser(NewUserViewModel newUserViewModel)
        {
            var claimInfo = new ClaimInfo();
            var partnerId = claimInfo.PartnerId();

            var dbRole = new PartnerWebSiteEntities();
            ViewBag.RoleList = new SelectList(dbRole.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name", newUserViewModel.RoleId);

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
                    ViewBag.RoleList = new SelectList(db.Role.Where(r => r.PartnerId == partnerId && r.IsEnabled).Select(r => new { Value = r.Id, Name = r.RoleName }).ToArray(), "Value", "Name");

                    var userViewModel = new UpdateUserRoleViewModel()
                    {
                        RoleId = (int)userAvaibleRole,
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
                    var validRole = db.Role.Where(r => r.Id == updateUserRoleViewModel.RoleId && r.IsEnabled).FirstOrDefault();
                    if (validRole != null)
                    {
                        var user = db.User.Find(updateUserRoleViewModel.UserId);

                        user.RoleId = updateUserRoleViewModel.RoleId;
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
                        var wrapper = new WebServiceWrapper();
                        Logger.Info("Updated User Role: " + user.UserSubMail + ", by: " + wrapper.GetUserSubMail());
                        //LOG

                        return RedirectToAction("Successful");
                    }
                    RedirectToAction("Index", "Home");
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
                        UserId = userId
                    };

                    return View(setupTeamStaffsToMatchedTheTask);
                }
                else
                {
                    TempData["GenericError"] = Localization.View.Generic200ErrorCodeMessage;
                    return RedirectToAction("SetupTeamList", "UserOperations");
                }
            }
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
                        return RedirectToAction("Index", "Home");
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
                        return RedirectToAction("Successful");
                    }
                    else
                    {
                        TempData["GenericError"] = Localization.View.Generic200ErrorCodeMessage;
                        return RedirectToAction("SetupTeamList", "UserOperations");
                    }
                }
            }
            setupTeamWorkingDaysAndHoursView.AvailableWorkingDays = UserWorkingDays(setupTeamWorkingDaysAndHoursView.UserId);
            return View(setupTeamWorkingDaysAndHoursView);

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
                        item.PermissionName = localizedList.GetDisplayText(item.PermissionId, CultureInfo.CurrentCulture);
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
                                RoleId = roleId
                            });
                            db.RolePermission.AddRange(addedRolePermission);
                            db.SaveChanges();


                            var currentUser = db.User.Where(u => u.RoleId == roleId && u.PartnerId == partnerId && u.IsEnabled).Select(u => u.Id).ToList();

                            if (currentUser.Count > 0)
                            {
                                if (!ValidRoleHaveSetupManagerPermission(roleId))
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

                                if (!ValidRoleHaveRendezvousTeamPermission(roleId))
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
                        }

                        return RedirectToAction("Successful");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = Localization.View.RequiredPermission;
            return RedirectToAction("UpdateRolePermission", new { roleId = roleId });
        }

        [Authorize(Roles = "Setup")]
        public ActionResult AddAndUpdateWorkAreaSetupTeamUser(int userId)
        {
            using (var db = new PartnerWebSiteEntities())
            {
                var user = db.SetupTeam.Find(userId);
                if (user != null && user.WorkingStatus == true)
                {
                    ViewBag.UserId = userId;
                    var wrapper = new WebServiceWrapper();
                    var provinceList = wrapper.GetProvince();

                    ViewBag.Provinces = new SelectList(provinceList.Data.ValueNamePairList.Select(nvpl => new { Name = nvpl.Name, Value = nvpl.Value }), "Value", "Name");
                    ViewBag.District = new SelectList("");
                    ViewBag.Rurals = new SelectList("");
                    ViewBag.Neigborhoods = new SelectList("");
                    //ViewBag.WorkAreaId = null;
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAndUpdateWorkAreaSetupTeamUser(WorkAreaSetupTeamUserViewModel workAreaSetupTeamUserViewModel)
        {
            var addressInfo = new AddressInfo();
            var provinceList = addressInfo.ProvincesList((long?)workAreaSetupTeamUserViewModel.ProvinceId ?? null);
            ViewBag.Provinces = provinceList;

            var districtList = addressInfo.DistrictList(workAreaSetupTeamUserViewModel.ProvinceId, workAreaSetupTeamUserViewModel.DistrictId ?? null);
            ViewBag.District = districtList;

            var ruralList = addressInfo.RuralRegionsList(workAreaSetupTeamUserViewModel.DistrictId ?? null, workAreaSetupTeamUserViewModel.RuralId ?? null);
            ViewBag.Rurals = ruralList;

            var neigborhoodList = addressInfo.NeighborhoodList(workAreaSetupTeamUserViewModel.RuralId ?? null, workAreaSetupTeamUserViewModel.NeigborhoodId ?? null);
            ViewBag.Neigborhoods = neigborhoodList;

            ViewBag.UserId = workAreaSetupTeamUserViewModel.UserId;
            ViewBag.WorkAreaId = null;

            if (ModelState.IsValid)
            {
                using (var db = new PartnerWebSiteEntities())
                {
                    //if (workAreaSetupTeamUserViewModel.WorkAreaId == null)//Add Operations
                    //{
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

                            return RedirectToAction("Successful");
                        }
                        ViewBag.ValidWWorkArea = Localization.View.ValidWorkArea;
                        return View(workAreaSetupTeamUserViewModel);
                    }
                    return RedirectToAction("Index", "Home");
                    //}
                    //else//Update Operations
                    //{
                    //    var validWorkArea = db.WorkArea.Where(wa => wa.UserId == workAreaSetupTeamUserViewModel.UserId && wa.Id == workAreaSetupTeamUserViewModel.WorkAreaId).FirstOrDefault();
                    //    if (validWorkArea != null)
                    //    {
                    //        if (validWorkArea.ProvinceId == workAreaSetupTeamUserViewModel.ProvinceId && validWorkArea.DistrictId == workAreaSetupTeamUserViewModel.DistrictId && validWorkArea.RuralId == workAreaSetupTeamUserViewModel.RuralId && validWorkArea.NeighbourhoodId == workAreaSetupTeamUserViewModel.NeigborhoodId)
                    //        {
                    //            return RedirectToAction("Successful");
                    //        }

                    //        var userOtherWorkAreas = db.WorkArea.Where(wa => wa.UserId == workAreaSetupTeamUserViewModel.UserId && wa.Id != workAreaSetupTeamUserViewModel.WorkAreaId).Select(wa => new { provinceId = wa.ProvinceId, districtId = wa.DistrictId, ruralId = wa.RuralId, neigborhoodId = wa.NeighbourhoodId });
                    //        var validUserWorkArea = userOtherWorkAreas.Where(uwa => uwa.provinceId == workAreaSetupTeamUserViewModel.ProvinceId && uwa.districtId == workAreaSetupTeamUserViewModel.DistrictId && uwa.ruralId == workAreaSetupTeamUserViewModel.RuralId && uwa.neigborhoodId == workAreaSetupTeamUserViewModel.NeigborhoodId).FirstOrDefault();
                    //        if (validUserWorkArea == null)
                    //        {
                    //            validWorkArea.ProvinceId = workAreaSetupTeamUserViewModel.ProvinceId;
                    //            validWorkArea.ProvinceName = workAreaSetupTeamUserViewModel.ProvinceName;
                    //            validWorkArea.DistrictId = workAreaSetupTeamUserViewModel.DistrictId ?? null;
                    //            validWorkArea.Districtname = workAreaSetupTeamUserViewModel.DistrictName;
                    //            validWorkArea.RuralId = workAreaSetupTeamUserViewModel.RuralId ?? null;
                    //            validWorkArea.RuralName = workAreaSetupTeamUserViewModel.RuralName;
                    //            validWorkArea.NeighbourhoodId = workAreaSetupTeamUserViewModel.NeigborhoodId ?? null;
                    //            validWorkArea.NeighbourhoodName = workAreaSetupTeamUserViewModel.NeigborhoodName;

                    //            db.SaveChanges();

                    //            //LOG
                    //            var wrapper = new WebServiceWrapper();
                    //            Logger.Info($"Updated WorkArea WorkAreaId: {validWorkArea.Id}, by: {wrapper.GetUserSubMail()}");
                    //            //LOG
                    //        }
                    //        else
                    //        {
                    //            ViewBag.ValidWWorkArea = Localization.View.ValidWorkArea;
                    //            return View(workAreaSetupTeamUserViewModel);
                    //        }
                    //        return RedirectToAction("Successful");
                    //    }
                    //    return RedirectToAction("Index", "Home");
                    //}

                }
            }

            return View(workAreaSetupTeamUserViewModel);
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

        //public ActionResult UpdateWorkAreaSetupTeamUser(long workAreaId)
        //{
        //    using (var db = new PartnerWebSiteEntities())
        //    {
        //        var workArea = db.WorkArea.Find(workAreaId);
        //        if (workArea != null)
        //        {
        //            var addressInfo = new AddressInfo();
        //            var provinceList = addressInfo.ProvincesList(workArea.ProvinceId);
        //            ViewBag.Provinces = provinceList;

        //            var districtList = addressInfo.DistrictList(workArea.ProvinceId, workArea.DistrictId ?? null);
        //            ViewBag.District = districtList;

        //            var ruralList = addressInfo.RuralRegionsList(workArea.DistrictId ?? null, workArea.RuralId ?? null);
        //            ViewBag.Rurals = ruralList;

        //            var neigborhoodList = addressInfo.NeighborhoodList(workArea.RuralId, workArea.NeighbourhoodId ?? null);
        //            ViewBag.Neigborhoods = neigborhoodList;

        //            ViewBag.UserId = workArea.UserId;

        //            ViewBag.WorkAreaId = workAreaId;

        //            return View("AddAndUpdateWorkAreaSetupTeamUser");
        //        }
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

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