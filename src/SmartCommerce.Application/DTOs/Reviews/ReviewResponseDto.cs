namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewResponseDto(
    Guid Id,
    Guid ProductId,
    Guid UserId,
    int Rating,
    string? Comment,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
