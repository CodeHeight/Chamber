using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class MembershipRoleMapping : EntityTypeConfiguration<MembershipRole>
    {
        public MembershipRoleMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.RoleName).IsRequired().HasMaxLength(50);
        }
    }
}