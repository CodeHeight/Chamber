using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services
{
    public partial class RoleService : IRoleService
    {
        private readonly ChamberContext _context;

        public RoleService(IChamberContext context)
        {
            _context = context as ChamberContext;
        }

        public IList<MembershipRole> AllRoles()
        {
            return _context.MembershipRole
                .OrderBy(x => x.RoleName)
                .ToList();
        }

        public void Delete(MembershipRole role)
        {
            // Check if anyone else if using this role
            var okToDelete = role.Users.Count == 0;

            if (okToDelete)
            {
                _context.MembershipRole.Remove(role);
            }
            else
            {
                var inUseBy = new List<Entity>();
                inUseBy.AddRange(role.Users);
                throw new InUseUnableToDeleteException(inUseBy);
            }
        }

        public MembershipRole GetRole(string rolename, bool removeTracking = false)
        {
            if (removeTracking)
            {
                return _context.MembershipRole
                    .AsNoTracking()
                    .FirstOrDefault(y => y.RoleName.Contains(rolename));
            }
            return _context.MembershipRole.FirstOrDefault(y => y.RoleName.Contains(rolename));
        }

        public MembershipRole GetRole(Guid id)
        {
            return _context.MembershipRole.FirstOrDefault(x => x.Id == id);
        }

        public IList<MembershipUser> GetUsersForRole(string roleName)
        {
            return GetRole(roleName).Users;
        }

        public MembershipRole CreateRole(MembershipRole role)
        {
            role.RoleName = StringUtils.SafePlainText(role.RoleName);
            var membershipRole = GetRole(role.RoleName);
            return membershipRole ?? _context.MembershipRole.Add(role);
        }
    }
}