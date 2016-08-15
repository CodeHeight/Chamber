using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data.Entity;
using System.Linq;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.Events;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services
{
    public partial class MembershipService : IMembershipService
    {
        private const int MaxHoursToResetPassword = 48;
        private readonly ChamberContext _context;
        private readonly IActivityService _activityService;
        private readonly ISettingsService _settingsService;

        public MembershipService(IChamberContext context, IActivityService activityService, ISettingsService settingsService)
        {
            _activityService = activityService;
            _settingsService = settingsService;
            _context = context as ChamberContext;
        }

        public MembershipUser Add(MembershipUser newUser)
        {
            return _context.MembershipUser.Add(newUser);
        }

        public string[] GetRolesForUser(string email)
        {
            email = StringUtils.SafePlainText(email);
            var roles = new List<string>();
            var user = GetUserByEmail(email, true);

            if (user != null)
            {
                roles.AddRange(user.Roles.Select(role => role.RoleName));
            }

            return roles.ToArray();
        }

        public MembershipUser GetUserByEmail(string email, bool removeTracking = false)
        {
            email = StringUtils.SafePlainText(email);
            if (removeTracking)
            {
                return _context.MembershipUser.AsNoTracking()
                    .Include(x => x.Roles)
                    .FirstOrDefault(y => y.Email == email);
            }
            return _context.MembershipUser
                .Include(x => x.Roles)
                .FirstOrDefault(y => y.Email == email);
        }

        public MembershipUser Get(Guid id)
        {
            return _context.MembershipUser
                .Include(x => x.Roles)
                .FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Remove the password reset security token and timestamp from the user record
        /// </summary>
        public bool ClearPasswordResetToken(MembershipUser user)
        {
            var existingUser = Get(user.Id);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.PasswordResetToken = null;
            existingUser.PasswordResetTokenCreatedAt = null;
            return true;
        }

        public MembershipCreateStatus CreateUser(MembershipUser newUser)
        {
            newUser = SanitizeUser(newUser);
            var settings = _settingsService.GetSettings(false);

            var status = MembershipCreateStatus.Success;

            var e = new RegisterUserEventArgs { User = newUser };     //fire event....left off here.  Admin 5/5/2016
            EventManager.Instance.FireBeforeRegisterUser(this, e);

            if (e.Cancel)
            {
                status = e.CreateStatus;
            }
            else
            {

                // Add get by email address
                if (GetUserByEmail(newUser.Email, true) != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                }

                if (string.IsNullOrEmpty(newUser.Password))
                {
                    status = MembershipCreateStatus.InvalidPassword;
                }

                if (status == MembershipCreateStatus.Success)
                {
                    // Hash the password
                    var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                    var hash = StringUtils.GenerateSaltedHash(newUser.Password, salt);
                    newUser.Password = hash;
                    newUser.PasswordSalt = salt;

                    newUser.Roles = new List<MembershipRole> { settings.NewMemberStartingRole };
                    // Set dates
                    newUser.CreateDate = DateTime.UtcNow;
                    newUser.LastLoginDate = DateTime.UtcNow;

                    //we will need this!.....(I think)
                    //-------------------------------
                    //var manuallyAuthoriseMembers = settings.ManuallyAuthoriseNewMembers;
                    //var memberEmailAuthorisationNeeded = settings.NewMemberEmailConfirmation == true;
                    //if (manuallyAuthoriseMembers || memberEmailAuthorisationNeeded)
                    //{
                    //    newUser.IsApproved = false;
                    //}
                    //else
                    //{
                    //    newUser.IsApproved = true;
                    //}

                    // url generator
                    newUser.Slug = ServiceHelpers.GenerateSlug(newUser.Email, GetUserBySlugLike(ServiceHelpers.CreateUrl(newUser.Email)), null);

                    try
                    {
                        Add(newUser);

                        //if (settings.EmailAdminOnNewMemberSignUp)
                        //{
                        //    var sb = new StringBuilder();
                        //    sb.AppendFormat("<p>{0}</p>", string.Format(_localizationService.GetResourceString("Members.NewMemberRegistered"), settings.ForumName, settings.ForumUrl));
                        //    sb.AppendFormat("<p>{0} - {1}</p>", newUser.UserName, newUser.Email);
                        //    var email = new Email
                        //    {
                        //        EmailTo = settings.AdminEmailAddress,
                        //        NameTo = _localizationService.GetResourceString("Members.Admin"),
                        //        Subject = _localizationService.GetResourceString("Members.NewMemberSubject")
                        //    };
                        //    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                        //    _emailService.SendMail(email);
                        //}

                        _activityService.MemberJoined(newUser);
                        EventManager.Instance.FireAfterRegisterUser(this,
                                                                    new RegisterUserEventArgs { User = newUser });
                    }
                    catch (Exception)
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
            }

            return status;
        }

        public MembershipCreateStatus AdminCreateUser(MembershipUser newUser, MembershipUser admin)
        {
            newUser = SanitizeUser(newUser);
            var settings = _settingsService.GetSettings(false);

            var status = MembershipCreateStatus.Success;

            var e = new AdminRegisterUserEventArgs { User = newUser, Admin = admin };     //fire event....left off here.  Admin 5/5/2016
            EventManager.Instance.FireBeforeAdminRegisterUser(this, e);

            if (e.Cancel)
            {
                status = e.CreateStatus;
            }
            else
            {

                // Add get by email address
                if (GetUserByEmail(newUser.Email, true) != null)
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                }

                if (string.IsNullOrEmpty(newUser.Password))
                {
                    //status = MembershipCreateStatus.InvalidPassword;
                    newUser.Password = "Password00$$!!";
                }

                if (status == MembershipCreateStatus.Success)
                {
                    // Hash the password
                    var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                    var hash = StringUtils.GenerateSaltedHash(newUser.Password, salt);
                    newUser.Password = hash;
                    newUser.PasswordSalt = salt;

                    newUser.Roles = new List<MembershipRole> { settings.NewMemberStartingRole };
                    // Set dates
                    newUser.CreateDate = DateTime.UtcNow;
                    newUser.LastLoginDate = DateTime.UtcNow;

                    //we will need this!.....(I think)
                    //-------------------------------
                    //var manuallyAuthoriseMembers = settings.ManuallyAuthoriseNewMembers;
                    //var memberEmailAuthorisationNeeded = settings.NewMemberEmailConfirmation == true;
                    //if (manuallyAuthoriseMembers || memberEmailAuthorisationNeeded)
                    //{
                    //    newUser.IsApproved = false;
                    //}
                    //else
                    //{
                    //    newUser.IsApproved = true;
                    //}

                    // url generator
                    newUser.Slug = ServiceHelpers.GenerateSlug(newUser.Email, GetUserBySlugLike(ServiceHelpers.CreateUrl(newUser.Email)), null);

                    try
                    {
                        Add(newUser);

                        //if (settings.EmailAdminOnNewMemberSignUp)
                        //{
                        //    var sb = new StringBuilder();
                        //    sb.AppendFormat("<p>{0}</p>", string.Format(_localizationService.GetResourceString("Members.NewMemberRegistered"), settings.ForumName, settings.ForumUrl));
                        //    sb.AppendFormat("<p>{0} - {1}</p>", newUser.UserName, newUser.Email);
                        //    var email = new Email
                        //    {
                        //        EmailTo = settings.AdminEmailAddress,
                        //        NameTo = _localizationService.GetResourceString("Members.Admin"),
                        //        Subject = _localizationService.GetResourceString("Members.NewMemberSubject")
                        //    };
                        //    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                        //    _emailService.SendMail(email);
                        //}

                        _activityService.AdminRegisterUser(newUser, admin);
                        EventManager.Instance.FireAfterAdminRegisterUser(this,
                                                                    new AdminRegisterUserEventArgs { User = newUser, Admin = admin });
                    }
                    catch (Exception)
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
            }
            return status;
        }

        public string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    return "Duplicate Email";

                case MembershipCreateStatus.InvalidPassword:
                    return "Invalid Password";

                case MembershipCreateStatus.InvalidEmail:
                    return "Invalid Email";

                case MembershipCreateStatus.UserRejected:
                    return "User Rejected";

                default:
                    return "Unknown Error";
            }
        }


        public IList<MembershipUser> GetUserBySlugLike(string slug)
        {
            return _context.MembershipUser
                    .Include(x => x.Roles)
                    .AsNoTracking()
                    .Where(name => name.Slug.ToUpper().Contains(slug.ToUpper()))
                    .ToList();
        }


        public MembershipUser SanitizeUser(MembershipUser membershipUser)
        {
            membershipUser.Email = StringUtils.SafePlainText(membershipUser.Email);
            membershipUser.Password = StringUtils.SafePlainText(membershipUser.Password);
            return membershipUser;
        }

        public MembershipUser CreateEmptyUser()
        {
            var now = DateTime.UtcNow;

            return new MembershipUser
            {
                Email = string.Empty,
                Password = string.Empty,
                CreateDate = now,
                LastLoginDate = (DateTime)SqlDateTime.MinValue,
                FirstName = string.Empty,
                LastName = string.Empty,
                City = string.Empty,
                State = string.Empty,
                DisplayName = string.Empty
            };
        }

        public IList<MembershipUser> GetAll()
        {
            var results = _context.MembershipUser
                .Include(x => x.Roles)
                .ToList();

            return results;
        }

        public PagedList<MembershipUser> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = _context.MembershipUser.Count();
            var results = _context.MembershipUser
                                .Include(x => x.Roles)
                                .OrderBy(x => x.Email)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new PagedList<MembershipUser>(results, pageIndex, pageSize, totalCount);
        }


        public PagedList<MembershipUser> SearchMembers(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.MembershipUser
                .Where(x => x.Email.ToUpper().Contains(search.ToUpper()) || x.Email.ToUpper().Contains(search.ToUpper()));

            var results = query
                .OrderBy(x => x.Email)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<MembershipUser>(results, pageIndex, pageSize, query.Count());
        }

        public PagedList<MembershipUser> SearchMembersByLastName(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.MembershipUser
                .Where(x => x.LastName.ToUpper().Contains(search.ToUpper()) || x.LastName.ToUpper().Contains(search.ToUpper()));

            var results = query
                .OrderBy(x => x.LastName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<MembershipUser>(results, pageIndex, pageSize, query.Count());
        }

        public IList<MembershipUser> SearchMembers(string username, int amount)
        {
            username = StringUtils.SafePlainText(username);
            return _context.MembershipUser
                .Where(x => x.Email.ToUpper().Contains(username.ToUpper()))
                .OrderBy(x => x.Email)
                .Take(amount)
                .ToList();
        }

        public LoginAttemptStatus LastLoginStatus { get; private set; } = LoginAttemptStatus.LoginSuccessful;

        public bool ValidateUser(string email, string password, int maxInvalidPasswordAttempts)
        {
            email = StringUtils.SafePlainText(email);
            password = StringUtils.SafePlainText(password);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUserByEmail(email);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.EmailNotFound;
                return false;
            }

            if (user.Active == false)
            {
                LastLoginStatus = LoginAttemptStatus.AccountNotActive;
                return false;
            }

            //if (user.IsBanned)
            //{
            //    LastLoginStatus = LoginAttemptStatus.Banned;
            //    return false;
            //}

            //if (user.IsLockedOut)
            //{
            //    LastLoginStatus = LoginAttemptStatus.UserLockedOut;
            //    return false;
            //}

            //if (!user.IsApproved)
            //{
            //    LastLoginStatus = LoginAttemptStatus.UserNotApproved;
            //    return false;
            //}

            //var allowedPasswordAttempts = maxInvalidPasswordAttempts;
            //if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            //{
            //    LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
            //    return false;
            //}

            var salt = user.PasswordSalt;
            var hash = StringUtils.GenerateSaltedHash(password, salt);
            var passwordMatches = hash == user.Password;

            //user.FailedPasswordAttemptCount = passwordMatches ? 0 : user.FailedPasswordAttemptCount + 1;

            //if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            //{
            //    user.IsLockedOut = true;
            //    user.LastLockoutDate = DateTime.UtcNow;
            //}

            if (!passwordMatches)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                return false;
            }

            return LastLoginStatus == LoginAttemptStatus.LoginSuccessful;
        }

        public bool ChangePassword(MembershipUser user, string oldPassword, string newPassword)
        {
            oldPassword = StringUtils.SafePlainText(oldPassword);
            newPassword = StringUtils.SafePlainText(newPassword);

            //n3oCacheHelper.Clear(user.UserName);
            var existingUser = Get(user.Id);
            var salt = existingUser.PasswordSalt;
            var oldHash = StringUtils.GenerateSaltedHash(oldPassword, salt);

            if (oldHash != existingUser.Password)
            {
                // Old password is wrong - do not allow update
                return false;
            }

            // Cleared to go ahead with new password
            salt = StringUtils.CreateSalt(AppConstants.SaltSize);
            var newHash = StringUtils.GenerateSaltedHash(newPassword, salt);

            existingUser.Password = newHash;
            existingUser.PasswordSalt = salt;
            existingUser.LastPasswordChangedDate = DateTime.UtcNow;

            return true;
        }



        public bool ResetPassword(MembershipUser user, string newPassword)
        {
            var existingUser = Get(user.Id);

            var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
            var newHash = StringUtils.GenerateSaltedHash(newPassword, salt);

            existingUser.Password = newHash;
            existingUser.PasswordSalt = salt;
            existingUser.LastPasswordChangedDate = DateTime.UtcNow;

            return true;
        }

        public bool UpdatePasswordResetToken(MembershipUser user)
        {
            var existingUser = Get(user.Id);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.PasswordResetToken = CreatePasswordResetToken();
            existingUser.PasswordResetTokenCreatedAt = DateTime.UtcNow;
            return true;
        }

        /// <summary>
        /// To be valid:
        /// - The user record must contain a password reset token
        /// - The given token must match the token in the user record
        /// - The token timestamp must be less than 24 hours ago
        /// </summary>
        public bool IsPasswordResetTokenValid(MembershipUser user, string token)
        {
            var existingUser = Get(user.Id);
            if (string.IsNullOrEmpty(existingUser?.PasswordResetToken))
            {
                return false;
            }
            // A security token must have an expiry date
            if (existingUser.PasswordResetTokenCreatedAt == null)
            {
                return false;
            }
            // The security token is only valid for 48 hours
            if ((DateTime.UtcNow - existingUser.PasswordResetTokenCreatedAt.Value).TotalHours >= MaxHoursToResetPassword)
            {
                return false;
            }
            return existingUser.PasswordResetToken == token;
        }

        public MembershipUser GetUserByDisplayName(string username, bool removeTracking = false)
        {
            MembershipUser member;

            if (removeTracking)
            {
                member = _context.MembershipUser
                    .Include(x => x.Roles)
                    .AsNoTracking()
                    .FirstOrDefault(name => name.DisplayName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                member = _context.MembershipUser
                    .Include(x => x.Roles)
                    .FirstOrDefault(name => name.DisplayName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            }


            // Do a check to log out the user if they are logged in and have been deleted
            //if (member == null && HttpContext.Current.User.Identity.Name == username)
            //{
            //    // Member is null so doesn't exist, yet they are logged in with that username - Log them out
            //    FormsAuthentication.SignOut();
            //}

            return member;
        }

        public void ProfileUpdated(MembershipUser user)
        {
            var e = new UpdateProfileEventArgs { User = user };
            EventManager.Instance.FireBeforeProfileUpdated(this, e);

            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterProfileUpdated(this, new UpdateProfileEventArgs { User = user });
                _activityService.ProfileUpdated(user);
            }
        }

        public void AdminProfileUpdated(MembershipUser user, MembershipUser admin)
        {
            var e = new AdminUpdateProfileEventArgs { User = user, Admin = admin };
            EventManager.Instance.FireBeforeAdminProfileUpdated(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterAdminProfileUpdated(this, new AdminUpdateProfileEventArgs { User = user, Admin = admin });
                _activityService.AdminProfileUpdated(user, admin);
            }
        }

        public bool DuplicateDisplayName(Guid memberId, string displayName)
        {
            displayName = StringUtils.SafePlainText(displayName);
            var user = Get(memberId);
            var query = _context.MembershipUser
                .AsNoTracking()
                .FirstOrDefault(name => name.DisplayName.Equals(displayName, StringComparison.CurrentCultureIgnoreCase)
                && name.DisplayName.ToUpper() != user.DisplayName.ToUpper());
            if (query == null)
            {
                return false;
            }
            return true;
        }


        private static string CreatePasswordResetToken()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }
    }
}