using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPulse.Domain.Enums;

namespace StockPulse.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string username, UserRole role);
    }

}
