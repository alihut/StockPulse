using Microsoft.EntityFrameworkCore;
using StockPulse.Domain.Entities;
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
        }
    }
}
