namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewResponseDto(
    string Id,
    string ProductId,
    string UserId,
    int Rating,
    string? Comment,
    DateTime CreatedAtUtc
);
