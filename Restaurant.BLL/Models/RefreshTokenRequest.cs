namespace Restaurant.BLL.Models
{
    public record RefreshTokenRequest(
       string Token,
       string RefreshToken
       );
}
