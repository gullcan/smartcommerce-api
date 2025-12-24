using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Infrastructure.Persistence;

namespace SmartCommerce.Infrastructure.Repositories;

public sealed class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public EfOrderRepository(AppDbContext db) => _db = db;

    public Task AddAsync(Order order, CancellationToken ct)
        => _db.Orders.AddAsync(order, ct).AsTask();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Orders.AsNoTracking()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, ct);

    public Task<Order?> GetForUpdateAsync(Guid id, CancellationToken ct)
        => _db.Orders.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
        => await _db.Orders.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await _db.Orders.AsNoTracking()
            .Where(x => !x.IsDeleted && x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
