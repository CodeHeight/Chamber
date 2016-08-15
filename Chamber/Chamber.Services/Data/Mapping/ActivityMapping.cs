using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel.Activity;

namespace Chamber.Services.Data.Mapping
{
    public class ActivityMapping : EntityTypeConfiguration<Activity>
    {
        public ActivityMapping()
        {
            HasKey(x => x.Id);

            Property(x => x.Id).IsRequired();
            Property(x => x.Type).IsRequired().HasMaxLength(50);
            Property(x => x.Data).IsRequired();
            Property(x => x.Timestamp).IsRequired();

            // TODO - Change Table Names
            //ToTable("Activity"); 
        }
    }
}