using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Infrastructure.Persistence;

namespace SmartCommerce.Infrastructure.Repositories;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public EfUserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var e = email.Trim().ToLowerInvariant();
        return _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == e, ct);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        var u = username.Trim();
        return _db.Users.FirstOrDefaultAsync(x => x.Username == u, ct);
    }

    public async Task<User?> GetByIdentifierAsync(string identifier, CancellationToken ct)
    {
        var trimmed = identifier.Trim();
        var lower = trimmed.ToLowerInvariant();

        // Email gibi görünüyorsa email ile ara, değilse username
        if (trimmed.Contains('@'))
            return await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == lower, ct);

        return await _db.Users.FirstOrDefaultAsync(x => x.Username == trimmed, ct);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        var e = email.Trim().ToLowerInvariant();
        return _db.Users.AnyAsync(x => x.Email.ToLower() == e, ct);
    }

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct)
    {
        var u = username.Trim();
        return _db.Users.AnyAsync(x => x.Username == u, ct);
    }

    public Task AddAsync(User user, CancellationToken ct)
        => _db.Users.AddAsync(user, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
