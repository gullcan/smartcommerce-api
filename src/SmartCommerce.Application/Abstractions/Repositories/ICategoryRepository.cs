using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Name bazında (trim + lower) var mı kontrol eder. excludeId verildiyse o kayıt hariç tutulur.
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken ct);

    Task AddAsync(Category category, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
