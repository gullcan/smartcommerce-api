namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewCreateDto(
    Guid ProductId,
    Guid UserId,
    int Rating,
    string? Comment
);
