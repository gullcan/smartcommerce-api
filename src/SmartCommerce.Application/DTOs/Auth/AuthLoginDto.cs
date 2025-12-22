namespace SmartCommerce.Application.DTOs.Auth;

public sealed record AuthLoginDto(
    string Identifier, // username OR email
    string Password
);
