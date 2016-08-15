using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class MembershipLevelMapping : EntityTypeConfiguration<MembershipLevel>
    {
        public MembershipLevelMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(200);
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.Description).IsOptional().HasMaxLength(1000);
            Property(x => x.EmployeeMax).IsOptional();
            Property(x => x.AnnualPrice).IsOptional();
            Property(x => x.SemiAnnualPrice).IsOptional();
            Property(x => x.Active).IsRequired();
        }
    }
}