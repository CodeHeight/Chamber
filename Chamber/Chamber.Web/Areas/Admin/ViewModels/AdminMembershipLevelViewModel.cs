using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class MembershipLevelViewModel
    {
        public Guid MembershipLevelId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 4)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
        public int? EmployeeMax { get; set; }
        public decimal? AnnualPrice { get; set; }
        public decimal? SemiAnnualPrice { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 4)]
        public string EditName { get; set; }

        [StringLength(1000)]
        public string EditDescription { get; set; }
        public int? EditEmployeeMax { get; set; }
        public decimal? EditAnnualPrice { get; set; }
        public decimal? EditSemiAnnualPrice { get; set; }

        public string Search { get; set; }


        public MembershipLevelListViewModel _listViewModel { get; set; }
    }

    public class MembershipLevelListViewModel
    {
        public PagedList<MembershipLevel> MembershipLevels { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<MembershipLevel> NonPagedMembershipLevels { get; set; }
    }

    public class AllMembershipLevelViewModel
    {
        public List<MembershipLevel> allMembershipLevels { get; set; }
    }
}