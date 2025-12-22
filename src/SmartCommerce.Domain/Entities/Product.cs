namespace SmartCommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Money
    public decimal Price { get; set; }

    // Inventory
    public int Stock { get; set; }

    // FK + Navigation
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
