namespace SmartCommerce.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;

    // JWT adımında gerçek hashing + auth akışı gelecek
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "User";

    // Navigations
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
