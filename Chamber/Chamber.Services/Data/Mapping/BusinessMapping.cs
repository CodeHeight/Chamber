using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class BusinessMapping : EntityTypeConfiguration<Business>
    {
        public BusinessMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired().HasMaxLength(200);
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.MailingAddress).IsOptional().HasMaxLength(100);
            Property(x => x.PhysicalAddress).IsOptional().HasMaxLength(100);
            Property(x => x.MailingCity).IsOptional().HasMaxLength(100);
            Property(x => x.MailingState).IsOptional().HasMaxLength(100);
            Property(x => x.MailingZipcode).IsOptional().HasMaxLength(5);
            Property(x => x.PhysicalCity).IsRequired().HasMaxLength(100);
            Property(x => x.PhysicalState).IsRequired().HasMaxLength(100);
            Property(x => x.PhysicalZipcode).IsOptional().HasMaxLength(5);
            Property(x => x.Active).IsRequired();
            Property(x => x.Completed).IsRequired();
            Property(x => x.CompletedDate).IsOptional();
            Property(x => x.Description).IsOptional().HasMaxLength(500);
            Property(x => x.Avatar).IsOptional().HasMaxLength(500);
            Property(x => x.Slug).IsOptional().HasMaxLength(100);
            Property(x => x.Phone).IsOptional().HasMaxLength(15);
            Property(x => x.WebAddress).IsOptional().HasMaxLength(100);
            HasOptional(x => x.User)
                .WithMany(x => x.Businesses)
                .Map(x => x.MapKey("User_Id"))
                .WillCascadeOnDelete(false);

            HasOptional(x => x.Classification)
                .WithMany(x => x.Businesses)
                .Map(x => x.MapKey("Classification_Id"))
                .WillCascadeOnDelete(false);

            HasOptional(x => x.MembershipLevel)
                .WithMany(x => x.Businesses)
                .Map(x => x.MapKey("MembershipLevel_Id"))
                .WillCascadeOnDelete(false);
        }
    }
}