namespace SmartCommerce.Application.Abstractions.Security;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(Guid userId, string username, string email, string role);
}
