using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;


namespace Chamber.Web.ViewModels
{
    public class BusinessViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Classification { get; set; }
        public string MembershipLevel { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalCity { get; set; }
        public string PhyscialState { get; set; }
        public string PhysicalZipcode { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public List<BusinessContact> businessContacts { get; set; }
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

    public class ClassificationListViewModel
    {
        public PagedList<Classification> AllClassifications { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<Business> AllBusinesses { get; set; }
    }

    public class ClassificationChildListViewModel
    {
        public PagedList<Business> allBusinesses { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public Guid ClassificationId { get; set; }
        public string ClassificationName { get; set; }
        public List<Business> AllBusinesses { get; set; }
    }

    public class BusinessContactListViewModel
    {
        public PagedList<BusinessContact> BusinessContacts { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<Business> NonPagedBusinesses { get; set; }
    }

    public class AllBusinessesViewModel
    {
        public List<Business> allBusinesses { get; set; }
    }

    public class StatesViewModel
    {
        public string state { get; set; }
        public List<State> allStates { get; set; }
    }
}