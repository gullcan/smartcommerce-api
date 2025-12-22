namespace SmartCommerce.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
