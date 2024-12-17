
using Restaurant.DAL.Entites.User;

public class Customer : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public List<RefreshToken> RefreshTokens { get; set; } = [];
}

