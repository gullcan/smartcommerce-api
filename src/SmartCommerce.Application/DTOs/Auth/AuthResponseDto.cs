namespace SmartCommerce.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    Guid UserId,
    string Username,
    string Email,
    string Role,
    string AccessToken,
    DateTime ExpiresAtUtc
);
