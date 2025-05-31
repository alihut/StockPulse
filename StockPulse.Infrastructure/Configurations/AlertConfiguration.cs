using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPulse.Domain.Entities;

namespace StockPulse.Infrastructure.Configurations;

public class AlertConfiguration : BaseEntityConfiguration<Alert>
{
    public override void Configure(EntityTypeBuilder<Alert> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.Symbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.PriceThreshold).IsRequired();

        builder.Property(a => a.Type).IsRequired().HasPrecision(18, 2);

        builder.Property(a => a.IsActive).HasDefaultValue(true);

        builder.HasCheckConstraint("CK_Alert_PriceThreshold_NonNegative", "[PriceThreshold] >= 0");
    }
}