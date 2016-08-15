using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Enums;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class MembershipUserViewModel
    {
        public Guid MembershipUserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        [StringLength(100, MinimumLength = 4)]
        public string City { get; set; }

        [StringLength(100, MinimumLength = 4)]
        public string State { get; set; }

        [StringLength(150, MinimumLength = 4)]
        public string DisplayName { get; set; }
        //public bool Active { get; set; }
        public string Role { get; set; }

        public HttpPostedFileBase[] Files { get; set; }
        public string Avatar { get; set; }
        public string RegisteredBusinessCount { get; set; }



        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string EditEmail { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string EditFirstName { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string EditLastName { get; set; }

        [StringLength(100, MinimumLength = 4)]
        public string EditCity { get; set; }

        [StringLength(100, MinimumLength = 4)]
        public string EditState { get; set; }

        [StringLength(150, MinimumLength = 4)]
        public string EditDisplayName { get; set; }
        public bool EditActive { get; set; }
        public string EditRole { get; set; }
        public string Search { get; set; }

        public MembershipUserListViewModel _listViewModel { get; set; }
        public StatesViewModel _statesViewModel { get; set; }
        public AdminMemberAddViewModel _adminMemberAddViewModel { get; set; }
    }


    public class MembershipUserListViewModel
    {
        public PagedList<MembershipUser> MembershipUsers { get; set; }
        public IList<MembershipRole> AllRoles { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<MembershipUser> NonPagedMembershipUsers { get; set; }
    }

    public class MemberRoleListViewModel
    {
        public IList<SingleMemberListViewModel> MembershipUsers { get; set; }
        public IList<MembershipRole> AllRoles { get; set; }
        public Guid Id { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
    }

    public class SingleMemberListViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string[] Roles { get; set; }
    }

    public class AllMembersListViewModel
    {
        public IList<SingleMemberListViewModel> AllMembershipUsersList { get; set; }
    }

    public class AdminMemberAddViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        public string[] Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public StatesViewModel _stateViewModel { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string ReturnUrl { get; set; }
        public string UserAccessToken { get; set; }
    }

    public class MembershipUserBusinessViewModel
    {
        public Guid MembershipUserId { get; set; }
        public string Role { get; set; }
        public IList<Business> UserBusinesses { get; set; }
        public string RegisteredBusinessCount { get; set; }
        public List<MembershipUser> NonPagedMembershipUsers { get; set; }

    }
}