using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IMembershipLevelService
    {
        MembershipLevel Add(MembershipLevel membershipLevel);
        void Delete(MembershipLevel membershipLevel);
        IList<MembershipLevel> GetAllMembershipLevels();
        PagedList<MembershipLevel> GetAll(int pageIndex, int pageSize);
        PagedList<MembershipLevel> Search(string search, int pageIndex, int pageSize);
        MembershipLevel GetByName(string name, bool tracking = false);
        MembershipLevel GetById(Guid id);
        MemberhipLevelCreateStatus CreateMembershipLevel(MembershipUser user, MembershipLevel membershipLevel);
        MembershipLevel SanitizeMembershipLevel(MembershipLevel membershipLevel);
        string ErrorCodeToString(MemberhipLevelCreateStatus createStatus);
        void MembershipLevelUpdated(MembershipUser user, MembershipLevel membershipLevel);
    }
}