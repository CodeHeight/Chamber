using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Domain.Interfaces.Services
{
    public enum LoginAttemptStatus
    {
        LoginSuccessful,
        EmailNotFound,
        PasswordIncorrect,
        AccountNotActive
    }

    public partial interface IMembershipService
    {
        MembershipUser Add(MembershipUser newUser);
        string[] GetRolesForUser(string username);
        MembershipUser GetUserByEmail(string email, bool removeTracking = false);
        MembershipUser Get(Guid id);
        MembershipCreateStatus CreateUser(MembershipUser newUser);
        MembershipCreateStatus AdminCreateUser(MembershipUser newUser, MembershipUser admin);
        string ErrorCodeToString(MembershipCreateStatus createStatus);
        MembershipUser SanitizeUser(MembershipUser membershipUser);
        MembershipUser CreateEmptyUser();
        IList<MembershipUser> GetAll();
        PagedList<MembershipUser> GetAll(int pageIndex, int pageSize);
        PagedList<MembershipUser> SearchMembers(string search, int pageIndex, int pageSize);
        PagedList<MembershipUser> SearchMembersByLastName(string search, int pageIndex, int pageSize);
        IList<MembershipUser> SearchMembers(string username, int amount);
        bool ValidateUser(string email, string password, int maxInvalidPasswordAttempts);
        LoginAttemptStatus LastLoginStatus { get; }
        bool ChangePassword(MembershipUser user, string oldPassword, string newPassword);
        bool ResetPassword(MembershipUser user, string newPassword);
        bool UpdatePasswordResetToken(MembershipUser user);
        bool IsPasswordResetTokenValid(MembershipUser user, string token);
        bool ClearPasswordResetToken(MembershipUser user);
        MembershipUser GetUserByDisplayName(string displayName, bool removeTracking = false);
        void ProfileUpdated(MembershipUser user);
        void AdminProfileUpdated(MembershipUser user, MembershipUser admin);
        bool DuplicateDisplayName(Guid memberId, string displayName);
    }
}
