namespace SmartCommerce.Application.DTOs.Products;

public sealed record ProductCreateDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId
);
