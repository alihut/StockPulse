using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPulse.Domain.Entities;

public class AlertConfiguration : BaseEntityConfiguration<Alert>
{
    public override void Configure(EntityTypeBuilder<Alert> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.Symbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.PriceThreshold).IsRequired();

        builder.Property(a => a.Type).IsRequired();

        builder.Property(a => a.IsActive).HasDefaultValue(true);
    }
}