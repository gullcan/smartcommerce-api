using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common.Exceptions;
using SmartCommerce.Application.DTOs.Categories;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var categories = await _repo.GetAllAsync(ct);
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(id, ct);
        if (category is null)
            throw new NotFoundException("Category not found.");

        return Map(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct)
    {
        var name = NormalizeName(dto.Name);
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Category name is required.");

        var conflict = await _repo.ExistsByNameAsync(name, excludeId: null, ct);
        if (conflict)
            throw new ConflictException("Category name already exists.");

        var entity = new Category
        {
            Name = name,
            Description = dto.Description?.Trim()
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<CategoryResponseDto> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(id, ct);
        if (category is null)
            throw new NotFoundException("Category not found.");

        var name = NormalizeName(dto.Name);
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Category name is required.");

        var conflict = await _repo.ExistsByNameAsync(name, excludeId: id, ct);
        if (conflict)
            throw new ConflictException("Category name already exists.");

        category.Name = name;
        category.Description = dto.Description?.Trim();

        await _repo.SaveChangesAsync(ct);

        return Map(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(id, ct);
        if (category is null)
            throw new NotFoundException("Category not found.");

        // Soft delete
        category.IsDeleted = true;

        await _repo.SaveChangesAsync(ct);
    }

    private static string NormalizeName(string name) => name.Trim();

    private static CategoryResponseDto Map(Category c) =>
        new CategoryResponseDto(
            c.Id,
            c.Name,
            c.Description,
            c.CreatedAt,
            c.UpdatedAt
        );
}
