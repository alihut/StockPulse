﻿using StockPulse.Domain.Enums;

namespace StockPulse.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public UserRole UserRole { get; set; } = UserRole.EndUser;
    }
}
