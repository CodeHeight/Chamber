using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Chamber.Domain.Constants;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.Application;
using Chamber.Web.Areas.Admin.ViewModels;
using MembershipCreateStatus = Chamber.Domain.DomainModel.MembershipCreateStatus;
using MembershipUser = Chamber.Domain.DomainModel.MembershipUser;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminMembershipUserController : BaseAdminController
    {
        private readonly IBusinessService _businessService;

        public AdminMembershipUserController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
          ISettingsService settingsService, IRoleService roleService, IBusinessService businessService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
            _businessService = businessService;
        }

        public ActionResult MembershipUser(int? p, string search)
        {
            var pageIndex = p ?? 1;

            var allMemberships = string.IsNullOrEmpty(search) ? MembershipService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                MembershipService.SearchMembers(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

            var listViewModel = new MembershipUserListViewModel
            {
                MembershipUsers = allMemberships,
                PageIndex = pageIndex,
                TotalCount = allMemberships.TotalCount,
                Search = search,
                TotalPages = allMemberships.TotalPages
            };

            StatesViewModel statesViewModel = new StatesViewModel()
            {
                allStates = SettingsService.ListOfStates().ToList()
            };

            var user = MembershipService.CreateEmptyUser();
            var adminViewModel = new AdminMemberAddViewModel
            {
                Email = user.Email,
                AllRoles = RoleService.AllRoles(),
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            adminViewModel._stateViewModel = statesViewModel;

            var viewModel = new MembershipUserViewModel
            {
                _listViewModel = listViewModel,
                _adminMemberAddViewModel = adminViewModel
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMembershipUser(AdminMemberAddViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                var admin = MembershipService.Get(loggedOnUserId);
                var settings = SettingsService.GetSettings();
                var homeRedirect = false;

                var userToSave = new MembershipUser
                {
                    Email = viewModel.Email,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    City = viewModel.City,
                    State = viewModel.State
                };
                var createStatus = MembershipService.AdminCreateUser(userToSave, admin);
                if (createStatus != MembershipCreateStatus.Success)
                {
                    ShowMessage(new AdminGenericMessageViewModel
                    {
                        Message = "Failed registering user: " + MembershipService.ErrorCodeToString(createStatus),
                        MessageType = GenericMessages.danger
                    });
                    return RedirectToAction("MembershipUser", "AdminMembershipUser", new { area = "Admin" });
                    //ModelState.AddModelError(string.Empty, MembershipService.ErrorCodeToString(createStatus));
                }
                SetRegisterViewBagMessage(false, false, userToSave);
                homeRedirect = true;
                try
                {
                    // Only send the email if the admin is not manually authorising emails or it's pointless
                    //SendEmailConfirmationEmail(userToSave);
                    unitOfWork.Commit();

                    if (homeRedirect)
                    {
                        if (Url.IsLocalUrl(viewModel.ReturnUrl) && viewModel.ReturnUrl.Length > 1 && viewModel.ReturnUrl.StartsWith("/")
                        && !viewModel.ReturnUrl.StartsWith("//") && !viewModel.ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(viewModel.ReturnUrl);
                        }
                        return RedirectToAction("MembershipUser", "AdminMembershipUser", new { area = "Admin" });
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    FormsAuthentication.SignOut();
                    ShowMessage(new AdminGenericMessageViewModel
                    {
                        Message = "Failed registering user.",
                        MessageType = GenericMessages.danger
                    });
                }
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
        }

        private void SetRegisterViewBagMessage(bool manuallyAuthoriseMembers, bool memberEmailAuthorisationNeeded, MembershipUser userToSave)
        {
            if (manuallyAuthoriseMembers)
            {
                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "An administrator will authorize you shortly",
                    MessageType = GenericMessages.success
                };
            }
            else if (memberEmailAuthorisationNeeded)
            {
                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "Check your email for authorization",
                    MessageType = GenericMessages.success
                };
            }
            else
            {
                // If not manually authorise then log the user in
                FormsAuthentication.SetAuthCookie(userToSave.Email, false);
                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "You are registered with Chamber's app",
                    MessageType = GenericMessages.success
                };
            }
        }

        public ActionResult BusinessList(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var membershipUser = MembershipService.Get(id);
                var membershipUserList = MembershipService.GetAll();
                var memberRole = MembershipService.GetRolesForUser(membershipUser.Email);
                var userBusiness = _businessService.GetUserBusinesses(membershipUser.Id);

                var viewModel = new MembershipUserBusinessViewModel
                {
                    MembershipUserId = membershipUser.Id,
                    Role = memberRole[0],
                    NonPagedMembershipUsers = membershipUserList.ToList()
                };

                if (membershipUser.Businesses != null)
                {
                    viewModel.RegisteredBusinessCount = membershipUser.Businesses.Count().ToString();
                }
                if (userBusiness != null)
                {
                    viewModel.UserBusinesses = userBusiness;
                }
                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpGet]
        public ActionResult EditMembershipUser(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var membershipUser = MembershipService.Get(id);
                var membershipUserList = MembershipService.GetAll();
                var memberRole = MembershipService.GetRolesForUser(membershipUser.Email);
                var listViewModel = new MembershipUserListViewModel
                {
                    NonPagedMembershipUsers = membershipUserList.ToList()
                };
                StatesViewModel statesViewModel = new StatesViewModel()
                {
                    allStates = SettingsService.ListOfStates().ToList()
                };


                var viewModel = new MembershipUserViewModel
                {
                    MembershipUserId = membershipUser.Id,
                    EditEmail = membershipUser.Email,
                    EditFirstName = membershipUser.FirstName,
                    EditLastName = membershipUser.LastName,
                    EditCity = membershipUser.City,
                    EditState = membershipUser.State,
                    EditDisplayName = membershipUser.DisplayName,
                    EditActive = membershipUser.Active,
                    Avatar = membershipUser.Avatar,
                    _listViewModel = listViewModel,
                    _statesViewModel = statesViewModel,
                    //add user details here
                    Role = memberRole[0]
                };
                if (membershipUser.Businesses != null)
                {
                    viewModel.RegisteredBusinessCount = membershipUser.Businesses.Count().ToString();
                }

                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpPost]
        public ActionResult EditMembershipUser(MembershipUserViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;     //Admin Id
                var admin = MembershipService.Get(loggedOnUserId);               //Admin doing update
                var user = MembershipService.Get(viewModel.MembershipUserId);    //User to update
                var membershipUserList = MembershipService.GetAll();             //Dashboard #'s
                var memberRole = MembershipService.GetRolesForUser(user.Email);  //User's role
                StatesViewModel statesViewModel = new StatesViewModel()          //Load static State dropdown
                {
                    allStates = SettingsService.ListOfStates().ToList()
                };
                viewModel._statesViewModel = statesViewModel;
                var listViewModel = new MembershipUserListViewModel             //Dashboard #'s
                {
                    NonPagedMembershipUsers = membershipUserList.ToList()
                };
                viewModel._listViewModel = listViewModel;
                viewModel.Role = memberRole[0];

                if (viewModel.EditDisplayName.Count() < 3)    //Display Name must have 3 Characters
                {
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Username must have atleast 3 characters.",
                        MessageType = GenericMessages.danger
                    };
                    return View(viewModel);
                }



                // Sort image out first
                if (viewModel.Files != null)
                {
                    // Before we save anything, check the user already has an upload folder and if not create one
                    var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, user.Id));
                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    // Loop through each file and get the file info and save to the users folder and Db
                    var file = viewModel.Files[0];
                    if (file != null)
                    {
                        // If successful then upload the file
                        var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, true);

                        if (!uploadResult.UploadSuccessful)
                        {
                            TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                            {
                                Message = uploadResult.ErrorMessage,
                                MessageType = GenericMessages.danger
                            };
                            return View(viewModel);
                        }
                        // Save avatar to user
                        user.Avatar = uploadResult.UploadedFileName;
                    }
                }

                // Set the users Avatar for the confirmation page
                viewModel.Avatar = user.Avatar;

                user.FirstName = viewModel.EditFirstName;
                user.LastName = viewModel.EditLastName;
                user.City = viewModel.EditCity;
                user.State = viewModel.EditState;

                if (viewModel.EditDisplayName != user.DisplayName)
                {
                    if (MembershipService.GetUserByDisplayName(viewModel.EditDisplayName) != null)
                    {
                        unitOfWork.Rollback();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "This display name is already taken. Please choose another display name.",
                            MessageType = GenericMessages.danger
                        };
                        return View(viewModel);
                    }
                    user.DisplayName = viewModel.EditDisplayName;
                }

                if (viewModel.EditEmail != user.Email)
                {
                    if (MembershipService.GetUserByEmail(viewModel.EditEmail) != null)
                    {
                        unitOfWork.Rollback();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "This email is already taken. Please choose another email.",
                            MessageType = GenericMessages.danger
                        };
                        return View(viewModel);
                    }
                    user.Email = viewModel.EditEmail;
                }

                try
                {
                    MembershipService.AdminProfileUpdated(user, admin);
                    unitOfWork.Commit();
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Profile updated",
                        MessageType = GenericMessages.success
                    };
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    //LoggingService.Error(ex);
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = ex.ToString(),
                        MessageType = GenericMessages.danger
                    };
                }
                return View(viewModel);
            }
        }
    }
}