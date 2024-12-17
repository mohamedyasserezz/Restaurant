using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.BLL.Models
{
    public record AuthResponse(
      string Id,
      string? Email,
      string Name,
      string Token,
      int ExpiresIn,
      string RefreshToken,
      DateTime RefreshTokenExpiration
      );
}
