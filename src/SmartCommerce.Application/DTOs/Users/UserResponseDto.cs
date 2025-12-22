namespace SmartCommerce.Application.DTOs.Users;

public sealed record UserResponseDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
