namespace Restaurant.DAL.Authentication
{
    public interface IJwtProvider
    {
        (string token, int ecpriesIn) GenerateToken(Customer user);
        string? ValidateToken(string token);
    }
}
