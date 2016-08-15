using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public Guid RoleId { get; set; }
        public List<MembershipRole> AllRoles { get; set; }
        public MembershipUserListViewModel _listViewModel { get; set; }
        public string Search { get; set; }
    }

    public class AjaxRoleUpdateViewModel
    {
        public Guid Id { get; set; }
        public string[] Roles { get; set; }
    }
}