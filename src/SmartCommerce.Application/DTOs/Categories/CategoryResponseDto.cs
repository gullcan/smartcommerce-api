namespace SmartCommerce.Application.DTOs.Categories;

public sealed record CategoryResponseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
