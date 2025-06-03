using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPulse.Application.Helpers;
using StockPulse.Domain.Entities;
using StockPulse.Domain.Enums;

namespace StockPulse.Infrastructure.Configurations;

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
        var password = "Password123";
        var passwordHash = HashHelper.HashPassword(password);
        var createdAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = createdAt,
                Username = "user1",
                PasswordHash = passwordHash
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CreatedAt = createdAt,
                Username = "user2",
                PasswordHash = passwordHash
            },
            new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CreatedAt = createdAt,
                Username = "user3",
                PasswordHash = passwordHash
            },
            new User
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                CreatedAt = createdAt,
                Username = "user4",
                PasswordHash = passwordHash
            },
            new User
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                CreatedAt = createdAt,
                Username = "user5",
                PasswordHash = passwordHash
            },
            new User
            {
                Id = Guid.Parse("70b0a12c-d1e4-404d-8e9e-6734101c8856"),
                CreatedAt = createdAt,
                Username = "admin",
                UserRole = UserRole.Admin,
                PasswordHash = passwordHash
            }
        };

        builder.HasData(users);
    }

}