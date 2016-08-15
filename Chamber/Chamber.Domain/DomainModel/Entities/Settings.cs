using System;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public partial class Settings : Entity
    {
        public Settings()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string Theme { get; set; }
        public bool RegistrationEnabled { get; set; }
        public virtual MembershipRole NewMemberStartingRole { get; set; }
    }
}