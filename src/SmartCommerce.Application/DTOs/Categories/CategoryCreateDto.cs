namespace SmartCommerce.Application.DTOs.Categories;

public sealed record CategoryCreateDto(
    string Name,
    string? Description
);
