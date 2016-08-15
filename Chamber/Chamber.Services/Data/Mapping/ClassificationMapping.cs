using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class ClassificationMapping : EntityTypeConfiguration<Classification>
    {
        public ClassificationMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(200);
            Property(x => x.Description).IsOptional().HasMaxLength(1000);
            Property(x => x.CreateDate).IsRequired();
        }
    }
}