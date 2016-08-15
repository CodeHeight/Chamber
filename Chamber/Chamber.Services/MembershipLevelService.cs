using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Domain.Events;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services
{
    public partial class MembershipLevelService : IMembershipLevelService
    {
        private readonly ChamberContext _context;
        private readonly IActivityService _activityService;

        public MembershipLevelService(IChamberContext context, IActivityService activityService)
        {
            _context = context as ChamberContext;
            _activityService = activityService;
        }

        public MembershipLevel Add(MembershipLevel membershipLevel)
        {
            membershipLevel.CreateDate = DateTime.UtcNow;
            membershipLevel.Active = true;
            return _context.MembershipLevel.Add(membershipLevel);
        }

        public void Delete(MembershipLevel membershipLevel)
        {
            _context.MembershipLevel.Remove(membershipLevel);
        }

        public IList<MembershipLevel> GetAllMembershipLevels()
        {
            return _context.MembershipLevel.ToList();
        }

        public PagedList<MembershipLevel> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = _context.MembershipLevel.Count();
            var results = _context.MembershipLevel
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<MembershipLevel>(results, pageIndex, pageSize, totalCount);
        }

        public PagedList<MembershipLevel> Search(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.MembershipLevel
                .Where(x => x.Name.ToUpper().Contains(search.ToUpper())
                || (x.Description.ToUpper().Contains(search.ToUpper())));

            var results = query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<MembershipLevel>(results, pageIndex, pageSize, query.Count());
        }

        public MembershipLevel GetByName(string name, bool tracking = false)
        {
            name = StringUtils.SafePlainText(name);
            if (tracking == false)
            {
                return _context.MembershipLevel.AsNoTracking().FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
            }
            return _context.MembershipLevel.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());
        }

        public MembershipLevel GetById(Guid id)
        {
            return _context.MembershipLevel.FirstOrDefault(x => x.Id == id);
        }

        public MemberhipLevelCreateStatus CreateMembershipLevel(MembershipUser user, MembershipLevel newMembershipLevel)
        {
            newMembershipLevel = SanitizeMembershipLevel(newMembershipLevel);
            var status = MemberhipLevelCreateStatus.Success;

            var e = new NewMembershipLevelEventArgs { User = user, MembershipLevel = newMembershipLevel };
            EventManager.Instance.FireBeforeNewMembershipLevel(this, e);
            if (e.Cancel)
            {
                status = e.CreateStatus;
            }
            else
            {
                if (string.IsNullOrEmpty(newMembershipLevel.Name))
                {
                    status = MemberhipLevelCreateStatus.InvalidName;
                }
                if (GetByName(newMembershipLevel.Name, true) != null)
                {
                    status = MemberhipLevelCreateStatus.DuplicateName;
                }
                if (status == MemberhipLevelCreateStatus.Success)
                {
                    try
                    {
                        Add(newMembershipLevel);
                        _activityService.MembershipLevelAdded(user, newMembershipLevel);
                        EventManager.Instance.FireAfterNewMembershipLevel(this,
                            new NewMembershipLevelEventArgs
                            {
                                User = user,
                                MembershipLevel = newMembershipLevel
                            });
                    }
                    catch (Exception)
                    {
                        status = MemberhipLevelCreateStatus.NameRejected;
                        //log error;
                    }
                }
            }
            return status;
        }

        public MembershipLevel SanitizeMembershipLevel(MembershipLevel membershipLevel)
        {
            membershipLevel.Name = StringUtils.SafePlainText(membershipLevel.Name);
            membershipLevel.Description = StringUtils.SafePlainText(membershipLevel.Description);
            return membershipLevel;
        }

        public string ErrorCodeToString(MemberhipLevelCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MemberhipLevelCreateStatus.DuplicateName:
                    return "Membership Level name is already in use.";
                case MemberhipLevelCreateStatus.InvalidName:
                    return "Invalid classification name";
                case MemberhipLevelCreateStatus.NameRejected:
                    return "Membership Level name was rejected.";
                default:
                    return "Membership Level Unknown error";
            }
        }

        public void MembershipLevelUpdated(MembershipUser user, MembershipLevel membershipLevel)
        {
            var e = new UpdateMembershipLevelEventArgs { User = user, MembershipLevel = membershipLevel };
            EventManager.Instance.FireBeforeUpdateMembershipLevel(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterUpdateMembershipLevel(this, new UpdateMembershipLevelEventArgs { User = user, MembershipLevel = membershipLevel });
                _activityService.MembershipLevelUpdated(user, membershipLevel);
            }
        }
    }
}