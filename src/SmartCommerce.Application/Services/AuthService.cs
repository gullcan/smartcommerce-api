using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Security;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common.Exceptions;
using SmartCommerce.Application.DTOs.Auth;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;

    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository users, IJwtTokenService jwt, IPasswordHasher hasher)
    {
        _users = users;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task<AuthResponseDto> RegisterAsync(AuthRegisterDto dto, CancellationToken ct)
    {
        var username = dto.Username.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Username is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            throw new ValidationException("Password must be at least 6 characters.");

        if (await _users.ExistsByUsernameAsync(username, ct))
            throw new ConflictException("Username already exists.");

        if (await _users.ExistsByEmailAsync(email, ct))
            throw new ConflictException("Email already exists.");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = "User"
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var token = _jwt.CreateToken(user.Id, user.Username, user.Email, user.Role);

        return new AuthResponseDto(user.Id, user.Username, user.Email, user.Role, token.AccessToken, token.ExpiresAtUtc);
    }

    public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto, CancellationToken ct)
    {
        var identifier = dto.Identifier.Trim();
        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ValidationException("Identifier and password are required.");

        var user = await _users.GetByIdentifierAsync(identifier, ct);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var ok = _hasher.Verify(dto.Password, user.PasswordHash);
        if (!ok)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwt.CreateToken(user.Id, user.Username, user.Email, user.Role);

        return new AuthResponseDto(user.Id, user.Username, user.Email, user.Role, token.AccessToken, token.ExpiresAtUtc);
    }
}
