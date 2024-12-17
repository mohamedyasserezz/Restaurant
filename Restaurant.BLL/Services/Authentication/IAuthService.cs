using Restaurant.BLL.Models;

namespace Restaurant.BLL.Services.Authentication
{
    public interface IAuthServices
    {
        Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

    }
}
