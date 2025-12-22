namespace SmartCommerce.Application.Abstractions.Security;

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);
