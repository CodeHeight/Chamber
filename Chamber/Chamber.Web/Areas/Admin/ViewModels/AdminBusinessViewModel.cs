using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class BusinessViewModel
    {
        public Guid BusinessId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }


        [StringLength(500, MinimumLength = 4)]
        public string Description { get; set; }
        public string Search { get; set; }

        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public StatesViewModel _stateViewModel { get; set; }

        public BusinessListViewModel _listViewModel { get; set; }
    }

    public class BusinessListViewModel
    {
        public PagedList<Business> Businesses { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<Business> NonPagedBusinesses { get; set; }
    }


    public class StatesViewModel
    {
        public string state { get; set; }
        public List<State> allStates { get; set; }
    }

    public class ListBooleanViewModel
    {
        public List<Bit> ListBoolean { get; set; }
    }

    public class BusinessContactViewModel
    {
        public Guid Id { get; set; }
        public Business business { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string Email { get; set; }
        public List<BusinessContact> businessContacts { get; set; }
    }

    public class BusinessDuesViewModel
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; }
        public decimal? AmountDue { get; set; }
        public decimal? AmountPaid { get; set; }
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DueDate { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? PaidDate { get; set; }
        public PagedList<BusinessBalance> AllDuesPaid { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class UpdateBusinessViewModel
    {
        public Guid Id { get; set; }
        public Guid Classification_Id { get; set; }
        public Guid MembershipLevel_Id { get; set; }
        public Guid MembershipUser_Id { get; set; }
        public string Name { get; set; }
        public string MailingAddress { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingZipcode { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public string PhysicalZipcode { get; set; }
        public bool Active { get; set; }
        public bool Completed { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public HttpPostedFileBase[] Files { get; set; }
        public ListBooleanViewModel _booleanViewModel { get; set; }
        public StatesViewModel _stateViewModel { get; set; }
        public AllClassificationsViewModel _allClassificationsViewModel { get; set; }
        public AllMembershipLevelViewModel _allMembershipLevelViewModel { get; set; }
        public AllMembersListViewModel _allMembersListViewModel { get; set; }
    }
}