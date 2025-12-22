namespace SmartCommerce.Application.DTOs.Users;

public sealed record UserCreateDto(
    string Username,
    string Email,
    string Password
);
