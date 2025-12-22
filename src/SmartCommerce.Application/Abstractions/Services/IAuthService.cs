using SmartCommerce.Application.DTOs.Auth;

namespace SmartCommerce.Application.Abstractions.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(AuthRegisterDto dto, CancellationToken ct);
    Task<AuthResponseDto> LoginAsync(AuthLoginDto dto, CancellationToken ct);
}
