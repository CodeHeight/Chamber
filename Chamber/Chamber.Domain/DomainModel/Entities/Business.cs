using System;
using Chamber.Utilities;
using System.Collections.Generic;

namespace Chamber.Domain.DomainModel
{
    public enum BusinessCreateStatus
    {
        Success,
        Incomplete,
        Rejected,
        DuplicateName
    }
    public partial class Business : Entity
    {
        public Business()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string MailingAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingZipcode { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public string PhysicalZipcode { get; set; }
        public bool Active { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Slug { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public virtual Classification Classification { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual MembershipLevel MembershipLevel { get; set; }
        public virtual IList<BusinessContact> BusinessContacts { get; set; }
        public virtual IList<BusinessBalance> BusinessBalances { get; set; }
    }
}
