using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class SettingsMapping : EntityTypeConfiguration<Settings>
    {
        public SettingsMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.SiteName).IsRequired().HasMaxLength(250);
            Property(x => x.SiteUrl).IsRequired().HasMaxLength(250);
            Property(x => x.Theme).IsRequired().HasMaxLength(50);
            Property(x => x.RegistrationEnabled).IsRequired();

            HasRequired(t => t.NewMemberStartingRole)
                .WithOptional(x => x.Settings).Map(m => m.MapKey("NewMemberStartingRole"));
        }
    }
}