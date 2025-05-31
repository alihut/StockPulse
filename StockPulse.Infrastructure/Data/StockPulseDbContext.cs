using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPulse.Domain.Entities;

namespace StockPulse.Infrastructure.Data
{
    public class StockPulseDbContext : DbContext
    {
        public StockPulseDbContext(DbContextOptions<StockPulseDbContext> options)
            : base(options) { }

        public DbSet<Alert> Alerts { get; set; }

        public DbSet<StockPrice> StockPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Alert>().HasKey(a => a.Id);
        }
    }
}
