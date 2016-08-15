using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Activity;
using Chamber.Domain.Interfaces;

namespace Chamber.Services.Data.Context
{
    public partial class ChamberContext : DbContext, IChamberContext
    {
        public ChamberContext()
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<BusinessBalance> BusinessBalance { get; set; }
        public DbSet<BusinessContact> BusinessContact { get; set; }
        public DbSet<Classification> Classification { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<MembershipLevel> MembershipLevel { get; set; }
        public DbSet<MembershipRole> MembershipRole { get; set; }
        public DbSet<MembershipUser> MembershipUser { get; set; }
        public DbSet<Settings> Setting { get; set; }
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // http://stackoverflow.com/questions/7924758/entity-framework-creates-a-plural-table-name-but-the-view-expects-a-singular-ta
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //Database.SetInitializer<MyDbContext>(null);
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                                    .Where(type => !string.IsNullOrEmpty(type.Namespace))
                                    .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                                    && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);

        }
    }
}