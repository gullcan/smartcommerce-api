using SmartCommerce.Application.DTOs.Categories;

namespace SmartCommerce.Application.Abstractions.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct);
    Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct);
    Task<CategoryResponseDto> UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
