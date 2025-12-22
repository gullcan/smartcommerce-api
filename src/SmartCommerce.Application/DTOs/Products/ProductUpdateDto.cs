namespace SmartCommerce.Application.DTOs.Products;

public sealed record ProductUpdateDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    Guid CategoryId
);
