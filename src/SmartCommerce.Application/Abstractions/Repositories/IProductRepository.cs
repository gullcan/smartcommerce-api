using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Aynı Category içinde aynı isimli ürün var mı kontrol eder (case-insensitive).
    /// excludeId verilirse o kayıt hariç tutulur.
    /// </summary>
    Task<bool> ExistsByNameInCategoryAsync(string name, Guid categoryId, Guid? excludeId, CancellationToken ct);

    Task AddAsync(Product product, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
