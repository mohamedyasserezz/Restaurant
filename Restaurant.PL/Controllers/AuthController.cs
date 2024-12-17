using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Restaurant.BLL.Models;
using Restaurant.BLL.Services.Authentication;
using Restaurant.DAL.Authentication;

namespace Restaurant.PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthServices authServices,
        IOptions<JwtOptions> options
        ) : ControllerBase
    {
        private readonly IAuthServices _authServices = authServices;
        private readonly JwtOptions options = options.Value;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            var response = await _authServices.RegisterAsync(registerRequest, cancellationToken);

            return response is null ? BadRequest("User registration failed or email already exists.") : Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var response = await _authServices.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);

            return response is null ? BadRequest() : Ok(response);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var response = await _authServices.GetRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);
            return response is null ? BadRequest() : Ok(response);
        }

        [HttpPost("Revoke-refresh-Token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var response = await _authServices.RevokeRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);
            return response is false ? BadRequest("Operation failed") : Ok(response);
        }


    }
}
