﻿using StockPulse.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StockPulse.Domain.Entities
{

    public class Alert : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Symbol { get; set; }
        public decimal PriceThreshold { get; set; }
        public AlertType Type { get; set; }
        public bool IsActive { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
