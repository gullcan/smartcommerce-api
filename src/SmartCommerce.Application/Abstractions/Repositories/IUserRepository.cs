using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<User?> GetByIdentifierAsync(string identifier, CancellationToken ct); // email or username

    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
