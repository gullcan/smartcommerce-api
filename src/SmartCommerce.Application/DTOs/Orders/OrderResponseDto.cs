using SmartCommerce.Domain.Enums;

namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderResponseDto(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    IReadOnlyList<OrderItemResponseDto> Items,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
