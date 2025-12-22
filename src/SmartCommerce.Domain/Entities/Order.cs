using SmartCommerce.Domain.Enums;

namespace SmartCommerce.Domain.Entities;

public class Order : BaseEntity
{
    // FK + Navigation
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Navigation
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
