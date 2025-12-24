using SmartCommerce.Application.Abstractions.Repositories;
using SmartCommerce.Application.Abstractions.Services;
using SmartCommerce.Application.Common.Exceptions;
using SmartCommerce.Application.DTOs.Orders;
using SmartCommerce.Domain.Entities;
using SmartCommerce.Domain.Enums;


namespace SmartCommerce.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;

    public OrderService(IOrderRepository orders, IProductRepository products, IUserRepository users)
    {
        _orders = orders;
        _products = products;
        _users = users;
    }

    public async Task<OrderResponseDto> CreateAsync(Guid userId, OrderCreateDto dto, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            throw new NotFoundException("User not found.");

        var now = DateTime.UtcNow;
        var items = new List<OrderItem>();
        var responseItems = new List<OrderItemResponseDto>();

        foreach (var it in dto.Items)
        {
            var pid = Guid.Parse(it.ProductId);

            var p = await _products.GetByIdAsync(pid, ct);
            if (p is null || p.IsDeleted)
                throw new NotFoundException($"Product not found: {pid}");

            if (it.Quantity > p.Stock)
                throw new ValidationException($"Insufficient stock for product: {p.Name}");

            p.Stock -= it.Quantity;
            p.UpdatedAt = now;

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = Guid.Empty,
                ProductId = p.Id,
                Quantity = it.Quantity,
                UnitPrice = p.Price,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            items.Add(orderItem);
            responseItems.Add(new OrderItemResponseDto(orderItem.Id, p.Id, p.Name, it.Quantity, p.Price));
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
            Items = items
        };

        foreach (var oi in items) oi.OrderId = order.Id;

        await _orders.AddAsync(order, ct);

        await _products.SaveChangesAsync(ct);
        await _orders.SaveChangesAsync(ct);

        return new OrderResponseDto(
            order.Id.ToString(),
            order.UserId.ToString(),
            order.Status.ToString(),
            order.CreatedAt,
            responseItems
        );
    }

    public async Task<IReadOnlyList<OrderResponseDto>> GetMineAsync(Guid userId, CancellationToken ct)
    {
        var list = await _orders.GetByUserIdAsync(userId, ct);
        return list.Select(o => new OrderResponseDto(
            o.Id.ToString(),
            o.UserId.ToString(),
            o.Status.ToString(),
            o.CreatedAt,
            Array.Empty<OrderItemResponseDto>()
        )).ToList();
    }

    public async Task<IReadOnlyList<OrderResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _orders.GetAllAsync(ct);
        return list.Select(o => new OrderResponseDto(
            o.Id.ToString(),
            o.UserId.ToString(),
            o.Status.ToString(),
            o.CreatedAt,
            Array.Empty<OrderItemResponseDto>()
        )).ToList();
    }

    public async Task<OrderResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var o = await _orders.GetByIdAsync(id, ct);
        if (o is null) throw new NotFoundException("Order not found.");

        return new OrderResponseDto(
            o.Id.ToString(),
            o.UserId.ToString(),
            o.Status.ToString(),
            o.CreatedAt,
            Array.Empty<OrderItemResponseDto>()
        );
    }

    public async Task<OrderResponseDto> UpdateStatusAsync(Guid id, string status, CancellationToken ct)
    {
        var o = await _orders.GetForUpdateAsync(id, ct);
        if (o is null) throw new NotFoundException("Order not found.");

        var s = status?.Trim();
        if (string.IsNullOrWhiteSpace(s))
            throw new ValidationException("Status is required.");

        if (!Enum.TryParse<OrderStatus>(s, ignoreCase: true, out var parsed))
            throw new ValidationException("Invalid status.");

        o.Status = parsed;
        o.UpdatedAt = DateTime.UtcNow;

        await _orders.SaveChangesAsync(ct);

        return new OrderResponseDto(
            o.Id.ToString(),
            o.UserId.ToString(),
            o.Status.ToString(),
            o.CreatedAt,
            Array.Empty<OrderItemResponseDto>()
        );
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var o = await _orders.GetForUpdateAsync(id, ct);
        if (o is null) throw new NotFoundException("Order not found.");

        o.IsDeleted = true;
        o.UpdatedAt = DateTime.UtcNow;

        await _orders.SaveChangesAsync(ct);
    }
}