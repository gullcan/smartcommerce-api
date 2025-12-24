namespace SmartCommerce.Application.DTOs.Orders;

public sealed record OrderResponseDto(
    string Id,
    string UserId,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemResponseDto> Items
);
