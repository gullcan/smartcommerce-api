namespace SmartCommerce.Domain.Entities;

public class Review : BaseEntity
{
    // FK + Navigation
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    // 1-5 (doğrulamayı service/validation adımında zorunlu yapacağız)
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
