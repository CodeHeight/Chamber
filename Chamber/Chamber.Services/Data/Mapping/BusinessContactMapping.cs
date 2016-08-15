using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class BusinessContactMapping : EntityTypeConfiguration<BusinessContact>
    {
        public BusinessContactMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            Property(x => x.LastName).IsRequired().HasMaxLength(50);
            Property(x => x.PrimaryPhone).IsOptional().HasMaxLength(12);
            Property(x => x.SecondaryPhone).IsOptional().HasMaxLength(12);
            Property(x => x.Email).IsOptional().HasMaxLength(100);
            Property(x => x.Active).IsRequired();
            Property(x => x.CreateDate).IsRequired();

            HasRequired(x => x.Business)
                .WithMany(x => x.BusinessContacts)
                .Map(x => x.MapKey("Business_Id"))
                .WillCascadeOnDelete(false);
        }
    }
}