using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Infrastructure.Persistence;

namespace SmartCommerce.Infrastructure.Repositories;

public sealed class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public EfProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Product>> GetAllAsync(CancellationToken ct)
        => _db.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByNameInCategoryAsync(string name, Guid categoryId, Guid? excludeId, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();

        var query = _db.Products.Where(x => x.CategoryId == categoryId);

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return query.AnyAsync(x => x.Name.ToLower() == normalized, ct);
    }

    public Task AddAsync(Product product, CancellationToken ct)
        => _db.Products.AddAsync(product, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
