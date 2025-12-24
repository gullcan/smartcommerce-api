using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Abstractions.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review, CancellationToken ct);
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct);
    Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
