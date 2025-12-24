using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Order?> GetForUpdateAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
