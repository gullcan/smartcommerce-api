namespace SmartCommerce.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;

    // JWT aşamasında geliştireceğiz
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User";
}
