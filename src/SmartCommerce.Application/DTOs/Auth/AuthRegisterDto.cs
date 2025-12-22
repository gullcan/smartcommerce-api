namespace SmartCommerce.Application.DTOs.Auth;

public sealed record AuthRegisterDto(
    string Username,
    string Email,
    string Password
);
