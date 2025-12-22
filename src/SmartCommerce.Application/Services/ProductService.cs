using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common.Exceptions;
using SmartCommerce.Application.DTOs.Products;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _products.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(id, ct);
        if (product is null)
            throw new NotFoundException("Product not found.");

        return Map(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken ct)
    {
        var name = NormalizeName(dto.Name);
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Product name is required.");

        if (dto.Price < 0)
            throw new ValidationException("Price cannot be negative.");

        if (dto.Stock < 0)
            throw new ValidationException("Stock cannot be negative.");

        // Category var mı?
        var category = await _categories.GetByIdAsync(dto.CategoryId, ct);
        if (category is null)
            throw new NotFoundException("Category not found.");

        // Aynı category içinde aynı isim var mı?
        var conflict = await _products.ExistsByNameInCategoryAsync(name, dto.CategoryId, excludeId: null, ct);
        if (conflict)
            throw new ConflictException("A product with the same name already exists in this category.");

        var entity = new Product
        {
            Name = name,
            Description = dto.Description?.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        await _products.AddAsync(entity, ct);
        await _products.SaveChangesAsync(ct);

        // Repo GetById include ile category doldurduğu için burada entity.Category null olabilir,
        // bu yüzden response için tek sefer daha okunabilir. Şimdilik basit tuttuk:
        entity.Category = category;

        return Map(entity);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(id, ct);
        if (product is null)
            throw new NotFoundException("Product not found.");

        var name = NormalizeName(dto.Name);
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Product name is required.");

        if (dto.Price < 0)
            throw new ValidationException("Price cannot be negative.");

        if (dto.Stock < 0)
            throw new ValidationException("Stock cannot be negative.");

        var category = await _categories.GetByIdAsync(dto.CategoryId, ct);
        if (category is null)
            throw new NotFoundException("Category not found.");

        var conflict = await _products.ExistsByNameInCategoryAsync(name, dto.CategoryId, excludeId: id, ct);
        if (conflict)
            throw new ConflictException("A product with the same name already exists in this category.");

        product.Name = name;
        product.Description = dto.Description?.Trim();
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;

        await _products.SaveChangesAsync(ct);

        product.Category = category;

        return Map(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(id, ct);
        if (product is null)
            throw new NotFoundException("Product not found.");

        product.IsDeleted = true;
        await _products.SaveChangesAsync(ct);
    }

    private static string NormalizeName(string name) => name.Trim();

    private static ProductResponseDto Map(Product p) =>
        new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.CategoryId,
            p.Category?.Name,
            p.CreatedAt,
            p.UpdatedAt
        );
}
