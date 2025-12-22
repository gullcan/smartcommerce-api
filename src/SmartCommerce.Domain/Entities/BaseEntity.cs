namespace SmartCommerce.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Bonus için hazır: Soft Delete
    public bool IsDeleted { get; set; } = false;
}
