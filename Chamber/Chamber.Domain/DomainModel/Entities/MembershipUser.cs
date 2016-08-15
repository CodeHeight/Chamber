using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public enum MembershipCreateStatus
    {
        Success,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        UserRejected
    }

    public partial class MembershipUser : Entity
    {
        public MembershipUser()
        {
            Id = GuidComb.GenerateComb();
            Active = true;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Slug { get; set; }
        public string Avatar { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenCreatedAt { get; set; }
        public bool Active { get; set; }

        public virtual IList<MembershipRole> Roles { get; set; }
        public virtual IList<Business> Businesses { get; set; }

        public string NiceUrl => UrlTypes.GenerateUrl(UrlType.Member, Slug);
    }
}
