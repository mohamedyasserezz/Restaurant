using Microsoft.AspNetCore.Identity;
using Restaurant.BLL.Models;
using Restaurant.DAL.Authentication;
using Restaurant.DAL.Entites.User;
using System.Security.Cryptography;
namespace Restaurant.BLL.Services.Authentication
{
    public class AuthServices(UserManager<Customer> userManager, IJwtProvider jwtProvider) : IAuthServices
    {
        private readonly UserManager<Customer> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;


        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var emailExist = await _userManager.FindByEmailAsync(request.Email);

            if (emailExist is not null)
                return null;

            var newUser = new Customer
            {
                Name = request.FirstName,
                UserName = request.UserName,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Code} - {error.Description}");
                }

                return null;
            }

            var (token, expiresIn) = _jwtProvider.GenerateToken(newUser);
            var refreshToken = GenerateRefreshToken();

            newUser.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
            });
            await _userManager.UpdateAsync(newUser);

            return new AuthResponse(
                newUser.Id,
                newUser.Email,
                newUser.Name,
                token,
                expiresIn * 60,
                refreshToken,
                DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
                );

        }
        public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result) return null;

            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),

            });
            await _userManager.UpdateAsync(user);
            return new AuthResponse(
                user.Id,
                user.Email,
                user.Name,
                token,
                expiresIn * 60,
                refreshToken,
                DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));
        }
        public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(U => U.Token == refreshToken && U.IsActive);

            if (userRefreshToken is null) return null;

            userRefreshToken.RevokedOn = DateTime.UtcNow;


            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

            var NewRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = NewRefreshToken,
                ExpiresOn = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),

            });
            await _userManager.UpdateAsync(user);

            return new AuthResponse(
                user.Id,
                user.Email,
                user.Name,
                newToken,
                expiresIn * 60,
                NewRefreshToken,
                DateTime.UtcNow.AddDays(_refreshTokenExpiryDays));
        }
        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);

            if (userId is null)
                return false;

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return false;
            var userRefreshToken = user.RefreshTokens.SingleOrDefault(R => R.Token == refreshToken && R.IsActive);

            if (userRefreshToken is null) return false;

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
            return true;

        }
        public static string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

       
    }
}
