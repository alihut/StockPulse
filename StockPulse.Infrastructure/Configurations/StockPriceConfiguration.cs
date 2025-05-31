using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPulse.Domain.Entities;

namespace StockPulse.Infrastructure.Configurations
{
    public class StockPriceConfiguration : BaseEntityConfiguration<StockPrice>
    {
        public override void Configure(EntityTypeBuilder<StockPrice> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(p => p.Price)
                .IsRequired().HasPrecision(18, 2);

            builder.HasCheckConstraint("CK_StockPrice_Price_NonNegative", "[Price] >= 0");
        }
    }

}
