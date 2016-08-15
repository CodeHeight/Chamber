using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using Chamber.Domain.DomainModel;

namespace Chamber.Services.Data.Mapping
{
    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Email).IsRequired().HasMaxLength(100);
            Property(x => x.Password).IsRequired().HasMaxLength(128);
            Property(x => x.PasswordSalt).IsOptional().HasMaxLength(128);
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.LastLoginDate).IsOptional();
            Property(x => x.FirstName).IsOptional().HasMaxLength(100);
            Property(x => x.LastName).IsOptional().HasMaxLength(100);
            Property(x => x.City).IsOptional().HasMaxLength(100);
            Property(x => x.State).IsOptional().HasMaxLength(100);
            Property(x => x.Slug).IsRequired().HasMaxLength(150)
                 .HasColumnAnnotation("Index",
                  new IndexAnnotation(new IndexAttribute("IX_MembershipUser_Slug", 1) { IsUnique = true }));
            Ignore(x => x.NiceUrl);
            Property(x => x.Avatar).IsOptional().HasMaxLength(500);
            Property(x => x.DisplayName).IsOptional().HasMaxLength(150);
            Property(x => x.LastPasswordChangedDate).IsOptional();
            Property(x => x.PasswordResetToken).IsOptional();
            Property(x => x.PasswordResetTokenCreatedAt).IsOptional();
            Property(x => x.Active).IsRequired();
            // Many-to-many join table - a user may belong to many roles
            HasMany(t => t.Roles)
            .WithMany(t => t.Users)
            .Map(m =>
            {
                m.ToTable("MembershipUsersInRoles");
                m.MapLeftKey("UserIdentifier");
                m.MapRightKey("RoleIdentifier");
            });
        }
    }
}