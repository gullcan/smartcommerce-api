using Microsoft.EntityFrameworkCore;
using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Infrastructure.Persistence;

namespace SmartCommerce.Infrastructure.Repositories;

public sealed class EfCategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public EfCategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Category>> GetAllAsync(CancellationToken ct)
        => _db.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Categories
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();

        var query = _db.Categories.AsQueryable();

        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);

        return query.AnyAsync(x => x.Name.ToLower() == normalized, ct);
    }

    public Task AddAsync(Category category, CancellationToken ct)
        => _db.Categories.AddAsync(category, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
