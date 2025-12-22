namespace SmartCommerce.Application.DTOs.Categories;

public sealed record CategoryUpdateDto(
    string Name,
    string? Description
);
