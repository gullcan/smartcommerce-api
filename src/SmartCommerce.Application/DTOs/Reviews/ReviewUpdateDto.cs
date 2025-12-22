namespace SmartCommerce.Application.DTOs.Reviews;

public sealed record ReviewUpdateDto(
    int Rating,
    string? Comment
);
