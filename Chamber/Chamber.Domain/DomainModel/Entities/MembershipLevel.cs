using System;
using System.Collections.Generic;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public enum MemberhipLevelCreateStatus
    {
        Success,
        DuplicateName,
        InvalidName,
        NameRejected
    }



    public partial class MembershipLevel : Entity
    {
        public MembershipLevel()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Description { get; set; }
        public int? EmployeeMax { get; set; }
        public decimal? AnnualPrice { get; set; }
        public decimal? SemiAnnualPrice { get; set; }
        public bool Active { get; set; }
        public IList<Business> Businesses { get; set; }
    }
}
