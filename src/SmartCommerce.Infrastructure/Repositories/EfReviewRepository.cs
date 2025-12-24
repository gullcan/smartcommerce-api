using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Infrastructure.Persistence;

namespace SmartCommerce.Infrastructure.Repositories;

public sealed class EfReviewRepository : IReviewRepository
{
    private readonly AppDbContext _db;
    public EfReviewRepository(AppDbContext db) => _db = db;

    public Task AddAsync(Review review, CancellationToken ct)
        => _db.Reviews.AddAsync(review, ct).AsTask();

    public Task<Review?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Reviews.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, ct);

    public Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct)
        => _db.Reviews.AnyAsync(x => !x.IsDeleted && x.UserId == userId && x.ProductId == productId, ct);

    public async Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken ct)
        => await _db.Reviews.AsNoTracking()
            .Where(x => !x.IsDeleted && x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
