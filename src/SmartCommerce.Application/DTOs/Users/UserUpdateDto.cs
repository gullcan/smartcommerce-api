namespace SmartCommerce.Application.DTOs.Users;

public sealed record UserUpdateDto(
    string Username,
    string Email,
    string? Role
);
