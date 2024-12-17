namespace Restaurant.DAL.Authentication
{
    public interface IJwtProvider
    {
        (string token, int ecpriesIn) GenerateToken(ApplicationUser user);
        string? ValidateToken(string token);
    }
}
