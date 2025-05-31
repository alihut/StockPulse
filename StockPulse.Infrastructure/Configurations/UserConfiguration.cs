using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPulse.Application.Helpers;
using StockPulse.Domain.Entities;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<User> builder)
    {
        var password = "Password123"; // Example shared password
        var passwordHash = HashHelper.HashPassword(password);

        var users = Enumerable.Range(1, 5).Select(i => new User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Username = $"user{i}",
            PasswordHash = passwordHash
        });

        builder.HasData(users);
    }
}