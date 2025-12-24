using SmartCommerce.Application.DTOs.Orders;

namespace SmartCommerce.Application.Abstractions.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(Guid userId, OrderCreateDto dto, CancellationToken ct);

    Task<IReadOnlyList<OrderResponseDto>> GetMineAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<OrderResponseDto>> GetAllAsync(CancellationToken ct);
    Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken ct);

    Task<OrderResponseDto> UpdateStatusAsync(Guid id, string status, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
