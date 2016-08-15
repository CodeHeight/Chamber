using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class BusinessBalanceMapping : EntityTypeConfiguration<BusinessBalance>
    {
        public BusinessBalanceMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.AmountDue).IsOptional();
            Property(x => x.AmountPaid).IsOptional();
            Property(x => x.Credit).IsOptional();
            Property(x => x.DueDate).IsOptional();
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.PaidDate).IsOptional();
            Property(x => x.Active).IsRequired();

            HasRequired(x => x.Business)
                .WithMany(x => x.BusinessBalances)
                .Map(x => x.MapKey("Business_Id"))
                .WillCascadeOnDelete(false);
        }
    }
}