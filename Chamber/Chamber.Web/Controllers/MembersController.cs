using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Enums;
using Chamber.Domain.Events;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Utilities;
using Chamber.Web.Application;
using Chamber.Web.ViewModels;
using MembershipCreateStatus = Chamber.Domain.DomainModel.MembershipCreateStatus;
using MembershipUser = Chamber.Domain.DomainModel.MembershipUser;

namespace Chamber.Web.Controllers
{
    public class MembersController : BaseController
    {
        private readonly IEmailService _emailService;

        public MembersController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            IRoleService roleService, ISettingsService settingsService, IEmailService emailService)
            : base(loggingService, unitOfWorkManager, membershipService, roleService, settingsService)
        {
            _emailService = emailService;
        }

        public ActionResult MemberDirectory()
        {
            return RedirectToAction("UnderDevelopment", "Home");
        }

        public FileResult DownloadMembershipForm()
        {
            string file = HostingEnvironment.MapPath("~/App_Data/Files/CoosaCountyChamberMembershipApplication_updateJuly15.pdf");
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(file));
        }

        public ActionResult OnlineMembershipForm()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var changePasswordSucceeded = true;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    var loggedOnUser = MembershipService.Get(LoggedOnReadOnlyUser.Id);
                    changePasswordSucceeded = MembershipService.ChangePassword(loggedOnUser, model.OldPassword, model.NewPassword);

                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        changePasswordSucceeded = false;
                    }
                }
            }

            // Commited successfully carry on
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (changePasswordSucceeded)
                {
                    // We use temp data because we are doing a redirect
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Your password was successfully updated!",
                        MessageType = GenericMessages.success
                    };
                    return View();
                }

                ModelState.AddModelError("", "Your password did not update.  Please try again because something went wrong.");
                return View(model);
            }

        }

        [Authorize]
        public ActionResult Edit()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser.Id;
                //var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                //var permissions = RoleService.GetPermissions(null, UsersRole);

                // Check is has permissions
                //if (UserIsAdmin || loggedOnUserId == id || permissions[SiteConstants.Instance.PermissionEditMembers].IsTicked)
                if (loggedOnUserId != null)
                {
                    var user = MembershipService.Get(loggedOnUserId);
                    var roles = MembershipService.GetRolesForUser(user.Email);
                    var viewModel = PopulateMemberViewModel(user);
                    viewModel.role = roles[0];
                    StatesViewModel statesViewModel = new StatesViewModel()
                    {
                        allStates = SettingsService.ListOfStates().ToList()
                    };
                    viewModel._stateViewModel = statesViewModel;
                    viewModel.user = user;
                    return View(viewModel);
                }

                return ErrorToHomePage("Updated profile is disabled for now");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(MemberFrontEndEditViewModel userModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                //var permissions = RoleService.GetPermissions(null, UsersRole);

                // Check is has permissions
                if (loggedOnUserId == userModel.Id) //|| permissions[SiteConstants.Instance.PermissionEditMembers].IsTicked)
                {
                    // Get the user from DB
                    var user = MembershipService.Get(userModel.Id);

                    StatesViewModel statesViewModel = new StatesViewModel()
                    {
                        allStates = SettingsService.ListOfStates().ToList()
                    };
                    userModel._stateViewModel = statesViewModel;

                    if (userModel.DisplayName.Count() < 3)
                    {
                        //Check if user name is taken *****
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Username must have atleast 3 characters.",
                            MessageType = GenericMessages.danger
                        });
                        return View(userModel);
                    }


                    // Sort image out first
                    if (userModel.Files != null)
                    {
                        // Before we save anything, check the user already has an upload folder and if not create one
                        var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, LoggedOnReadOnlyUser.Id));
                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }

                        // Loop through each file and get the file info and save to the users folder and Db
                        var file = userModel.Files[0];
                        if (file != null)
                        {
                            // If successful then upload the file
                            var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, true);

                            if (!uploadResult.UploadSuccessful)
                            {
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = uploadResult.ErrorMessage,
                                    MessageType = GenericMessages.danger
                                };
                                return View(userModel);
                            }

                            // Save avatar to user
                            user.Avatar = uploadResult.UploadedFileName;
                        }
                    }

                    // Set the users Avatar for the confirmation page
                    userModel.Avatar = user.Avatar;

                    // Update other users properties
                    //user.Location = _bannedWordService.SanitiseBannedWords(userModel.City, bannedWords);
                    user.City = userModel.City;
                    user.State = userModel.State;

                    // User is trying to change username, need to check if a user already exists
                    // with the username they are trying to change to
                    var changedUsername = false;
                    //var sanitisedUsername = _bannedWordService.SanitiseBannedWords(userModel.UserName, bannedWords);

                    if (userModel.DisplayName != user.DisplayName)
                    {
                        if (MembershipService.GetUserByDisplayName(userModel.DisplayName) != null)
                        {
                            unitOfWork.Rollback();
                            ModelState.AddModelError(string.Empty, "Duplicate Display Name");
                            return View(userModel);
                        }

                        user.DisplayName = userModel.DisplayName;
                        changedUsername = true;
                    }

                    // User is trying to update their email address, need to 
                    // check the email is not already in use
                    //if (userModel.Email != user.Email)
                    //{
                    //    // Add get by email address
                    //    if (MembershipService.GetUserByEmail(userModel.Email) != null)
                    //    {
                    //        unitOfWork.Rollback();
                    //        ModelState.AddModelError(string.Empty, "Duplicate Email.");
                    //        return View(userModel);
                    //    }
                    //    user.Email = userModel.Email;
                    //}


                    try
                    {
                        MembershipService.ProfileUpdated(user);

                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Profile updated",
                            MessageType = GenericMessages.success
                        });

                        if (changedUsername)  //This is email (not implimented yet....)
                        {
                            // User has changed their username so need to log them in
                            // as there new username of 
                            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                            if (authCookie != null)
                            {
                                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                                if (authTicket != null)
                                {
                                    var newFormsIdentity = new FormsIdentity(new FormsAuthenticationTicket(authTicket.Version,
                                                                                                           user.Email,
                                                                                                           authTicket.IssueDate,
                                                                                                           authTicket.Expiration,
                                                                                                           authTicket.IsPersistent,
                                                                                                           authTicket.UserData));
                                    var roles = authTicket.UserData.Split("|".ToCharArray());
                                    var newGenericPrincipal = new GenericPrincipal(newFormsIdentity, roles);
                                    System.Web.HttpContext.Current.User = newGenericPrincipal;
                                }
                            }

                            // sign out current user
                            FormsAuthentication.SignOut();

                            // Abandon the session
                            Session.Abandon();

                            // Sign in new user
                            FormsAuthentication.SetAuthCookie(user.Email, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError(string.Empty, "Error updating profile");
                    }
                    return View(userModel);
                }
                return ErrorToHomePage("Profile disabled");
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            MembershipUser user;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                user = MembershipService.GetUserByEmail(viewModel.EmailAddress);
                if (user == null)
                {
                    return RedirectToAction("PasswordResetSent", "Members");
                }

                try
                {
                    // If the user is registered then create a security token and a timestamp that will allow a change of password
                    MembershipService.UpdatePasswordResetToken(user);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    ModelState.AddModelError("", "Reset password error");
                    return View(viewModel);
                }
            }

            // At this point the email address is registered and a security token has been created
            // so send an email with instructions on how to change the password
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var url = new Uri(string.Concat(settings.SiteUrl.TrimEnd('/'), Url.Action("ResetPassword", "Members", new { user.Id, token = user.PasswordResetToken })));

                var sb = new StringBuilder();
                sb.AppendFormat("<p>{0}</p>", string.Format("Put body text here.", settings.SiteUrl));
                sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", url);

                var email = new Email
                {
                    EmailTo = user.Email,
                    NameTo = user.FirstName + " " + user.LastName,
                    Subject = "Forgot password from Chamber",
                };
                email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                _emailService.SendMail(email);
                //"<table width=100%'><tbody><tr><td align='center'><table width='640' cellpadding='0' cellspacing='0' border='0'><tbody><tr><td colspan='3' width='640' height='25'></td></tr><tr> <td width='30'></td><td width='580'> <table width='580' cellpadding='0' cellspacing='0' border='0'> <tbody> <tr> <td width='580'> <div align='left' class='article - content'> <p><p>joshyates1980,</p></p><p>A request has been made to reset your password on Southern Star Technology. To reset your password follow the link below. If you did not make this request then please ignore this email. No further action is required and your password will not be changed.</p><p><a href='http://www.southernstartech.us/members/resetpassword/df30b7f4-49b8-4267-a9f4-a5a001445eb1/?token=a1d59458240d4fb6b799f1f6637753fb'>http://www.southernstartech.us/members/resetpassword/df30b7f4-49b8-4267-a9f4-a5a001445eb1/?token=a1d59458240d4fb6b799f1f6637753fb</a></p> <p>&nbsp;</p><a href='http://www.southernstartech.us'>http://www.southernstartech.us</a> </div></td></tr></tbody> </table> </td><td width='30'></td></tr><tr> <td colspan='3' width='640' height='5'></td></tr><tr> <td colspan='3' width='640' height='5'></td></tr><tr> <td colspan='3' width='640' height='25'></td></tr></tbody> </table> </td></tr></tbody></table></body></html>"
                string api_user = "sendgrid-api goes here";
                string api_key = "sendgrid key goes here";
                string toAddress = email.EmailTo;
                string toName = email.NameTo;
                string subject = email.Subject;
                string text = email.Body;
                string fromAddress = "do-not-reply@southernstartech.us";
                string url2 = "https://sendgrid.com/api/mail.send.json";
                // Create a form encoded string for the request body
                string parameters = "api_user=" + api_user + "&api_key=" + api_key + "&to=" + toAddress +
                                    "&toname=" + toName + "&subject=" + subject + "&html=" + "<table width=100%'><tbody><tr><td align='center'><table width='640' cellpadding='0' cellspacing='0' border='0'><tbody><tr><td colspan='3' width='640' height='25'></td></tr><tr> <td width='30'></td><td width='580'> <table width='580' cellpadding='0' cellspacing='0' border='0'> <tbody> <tr> <td width='580'> <div align='left' class='article - content'> <p><p>joshyates1980,</p></p><p>A request has been made to reset your password on Southern Star Technology. To reset your password follow the link below. If you did not make this request then please ignore this email. No further action is required and your password will not be changed.</p><p><a href='" + url + "'>Click Here</a></p> <p>&nbsp;</p><a href='https://www.southernstartech.us'>https://www.southernstartech.us</a> </div></td></tr></tbody> </table> </td><td width='30'></td></tr><tr> <td colspan='3' width='640' height='5'></td></tr><tr> <td colspan='3' width='640' height='5'></td></tr><tr> <td colspan='3' width='640' height='25'></td></tr></tbody> </table> </td></tr></tbody></table></body></html>" +
                                    "&from=" + fromAddress;
                //Create Request
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url2);
                myHttpWebRequest.Method = "POST";
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

                // Create a new write stream for the POST body
                StreamWriter streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream());

                // Write the parameters to the stream
                streamWriter.Write(parameters);
                streamWriter.Flush();
                streamWriter.Close();

                // Get the response
                HttpWebResponse httpResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                // Create a new read stream for the response body and read it
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                string result = streamReader.ReadToEnd();
                try
                {
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    ModelState.AddModelError("", "Error sending email on forgot password page");
                    return View(viewModel);
                }
            }

            return RedirectToAction("PasswordResetSent", "Members");
        }

        public ActionResult Logout()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                FormsAuthentication.SignOut();
                ViewBag.Message = new GenericMessageViewModel
                {
                    Message = "You have successfully logged out of the Chamber app",
                    MessageType = GenericMessages.success
                };
                return RedirectToAction("Index", "Home", new { area = string.Empty });
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            var viewModel = new LoginViewModel();
            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var email = viewModel.Email;
                var password = viewModel.Password;

                try
                {
                    if (ModelState.IsValid)
                    {
                        var e = new LoginEventArgs
                        {
                            Email = viewModel.Email,
                            Password = viewModel.Password,
                            RememberMe = viewModel.RememberMe,
                            ReturnUrl = viewModel.ReturnUrl,
                            UnitOfWork = unitOfWork
                        };
                        EventManager.Instance.FireBeforeLogin(this, e);

                        if (!e.Cancel)
                        {
                            var message = new GenericMessageViewModel();
                            var user = new MembershipUser();
                            if (MembershipService.ValidateUser(email, password, 0))
                            {
                                user = MembershipService.GetUserByEmail(email);
                                //if (user.IsApproved && !user.IsLockedOut && !user.IsBanned)
                                //{
                                FormsAuthentication.SetAuthCookie(email, viewModel.RememberMe);
                                user.LastLoginDate = DateTime.UtcNow;

                                if (Url.IsLocalUrl(viewModel.ReturnUrl) && viewModel.ReturnUrl.Length > 1 && viewModel.ReturnUrl.StartsWith("/")
                                    && !viewModel.ReturnUrl.StartsWith("//") && !viewModel.ReturnUrl.StartsWith("/\\"))
                                {
                                    return Redirect(viewModel.ReturnUrl);
                                }

                                message.Message = "You have successfully logged into the Chamber app";
                                message.MessageType = GenericMessages.success;
                                TempData[AppConstants.MessageViewBagName] = message;

                                EventManager.Instance.FireAfterLogin(this, new LoginEventArgs
                                {
                                    Email = viewModel.Email,
                                    Password = viewModel.Password,
                                    RememberMe = viewModel.RememberMe,
                                    ReturnUrl = viewModel.ReturnUrl,
                                    UnitOfWork = unitOfWork
                                });

                                return RedirectToAction("Index", "Home", new { area = string.Empty });
                            }
                            else
                            {
                                message.Message = "Something happened and your login failed.  Try again.";
                                message.MessageType = GenericMessages.danger;
                                TempData[AppConstants.MessageViewBagName] = message;
                                return View();
                            }
                        }
                    }
                }
                finally
                {
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                    }

                }
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (SettingsService.GetSettings().RegistrationEnabled == true)
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.CreateEmptyUser();
                    var viewModel = new MemberAddViewModel
                    {
                        Email = user.Email,
                        Password = user.Password,
                        AllRoles = RoleService.AllRoles(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        City = user.City,
                        State = user.State
                    };
                    StatesViewModel statesViewModel = new StatesViewModel()
                    {
                        allStates = SettingsService.ListOfStates().ToList()
                    };
                    viewModel._stateViewModel = statesViewModel;

                    // See if a return url is present or not and add it
                    var returnUrl = Request["ReturnUrl"];
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        viewModel.ReturnUrl = returnUrl;
                    }
                    return View(viewModel);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MemberAddViewModel viewModel)
        {
            if (SettingsService.GetSettings().RegistrationEnabled == true)
            {
                // Do the register logic
                return MemberRegisterLogic(viewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MemberRegisterLogic(MemberAddViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var homeRedirect = false;

                var userToSave = new MembershipUser
                {
                    Email = viewModel.Email,
                    Password = viewModel.Password,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    City = viewModel.City,
                    State = viewModel.State
                };
                var createStatus = MembershipService.CreateUser(userToSave);
                if (createStatus != MembershipCreateStatus.Success)
                {
                    ModelState.AddModelError(string.Empty, MembershipService.ErrorCodeToString(createStatus));
                }
                else
                {
                    var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, userToSave.Id));
                    if (uploadFolderPath != null && !Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    //admin/email approval goes here
                    SetRegisterViewBagMessage(false, false, userToSave);
                    //if (!manuallyAuthoriseMembers && !memberEmailAuthorisationNeeded)
                    //{
                    homeRedirect = true;
                    //}
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
                            return RedirectToAction("Index", "Home", new { area = string.Empty });
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        FormsAuthentication.SignOut();
                        ModelState.AddModelError(string.Empty, "Error registering");
                    }
                }
            }
            return View("Register");
        }

        [HttpGet]
        public ViewResult ResetPassword(Guid? id, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Id = id,
                Token = token
            };

            if (id == null || String.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Invalid token for reseting your password.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel postedModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postedModel);
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (postedModel.Id != null)
                {
                    var user = MembershipService.Get(postedModel.Id.Value);

                    // if the user id wasn't found then we can't proceed
                    // if the token submitted is not valid then do not proceed
                    if (user == null || user.PasswordResetToken == null || !MembershipService.IsPasswordResetTokenValid(user, postedModel.Token))
                    {
                        ModelState.AddModelError("", "Invalid token for reseting your password.");
                        return View(postedModel);
                    }

                    try
                    {
                        // The security token is valid so change the password
                        MembershipService.ResetPassword(user, postedModel.NewPassword);
                        // Clear the token and the timestamp so that the URL cannot be used again
                        MembershipService.ClearPasswordResetToken(user);
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError("", "Invalid token for reseting your password.");
                        return View(postedModel);
                    }
                }
            }

            return RedirectToAction("PasswordChanged", "Members");
        }

        private void SetRegisterViewBagMessage(bool manuallyAuthoriseMembers, bool memberEmailAuthorisationNeeded, MembershipUser userToSave)
        {
            if (manuallyAuthoriseMembers)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "An administrator will authorize you shortly",
                    MessageType = GenericMessages.success
                };
            }
            else if (memberEmailAuthorisationNeeded)
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "Check your email for authorization",
                    MessageType = GenericMessages.success
                };
            }
            else
            {
                // If not manually authorise then log the user in
                FormsAuthentication.SetAuthCookie(userToSave.Email, false);
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "You are registered with Chamber's app",
                    MessageType = GenericMessages.success
                };
            }
        }

        private static MemberFrontEndEditViewModel PopulateMemberViewModel(MembershipUser user)
        {
            var viewModel = new MemberFrontEndEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                State = user.State,
                Avatar = user.Avatar
            };

            return viewModel;
        }
    }
}