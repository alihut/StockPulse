using Microsoft.EntityFrameworkCore;
using StockPulse.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace StockPulse.Infrastructure.Data
{
    public class StockPulseDbContext : DbContext
    {
        public StockPulseDbContext(DbContextOptions<StockPulseDbContext> options)
            : base(options) { }

        public DbSet<Alert> Alerts { get; set; }

        public DbSet<StockPrice> StockPrices { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ApplySoftDeleteQueryFilters(modelBuilder);
        }

        private void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Equal(prop, Expression.Constant(false)), parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
