using SmartCommerce.Application.DTOs.Products;

namespace SmartCommerce.Application.Abstractions.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponseDto>> GetAllAsync(CancellationToken ct);
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken ct);
    Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
