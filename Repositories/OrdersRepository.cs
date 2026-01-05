using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly PixelMartDbContext _dbContext;

    public OrdersRepository(PixelMartDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        var orders = await _dbContext.Orders
            .Include(sc => sc.Items)
            .AsNoTracking()
            .ToListAsync();

        return orders;
    }

    public async Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId)
    {
        var userOrders = await _dbContext.Orders
            .Include(sc => sc.Items)
            .Where(o => o.UserId == userId.ToString())
            .AsNoTracking()
            .ToListAsync();

        return userOrders;
    }

    public Task<Order?> GetOrderForUserAsync(Guid userId, Guid orderId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        var order = _dbContext.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId.ToString() && o.Id == orderId);

        return order == null ? throw new InvalidOperationException("Order not found for user.") : order!;
    }

    public async Task CreateOrderAsync(Guid userId, Order order)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(order);

        order.UserId = userId.ToString();
        await _dbContext.Orders.AddAsync(order);
    }

    public async Task UpdateOrderAsync(Guid userId, Guid orderId, Order orderUpdated)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        ArgumentNullException.ThrowIfNull(orderUpdated);

        var userOrder = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.UserId == userId.ToString() && o.Id == orderId);

        if (userOrder is null)
            throw new InvalidOperationException("Order not found for user.");

        // Update scalar properties
        userOrder.Status = orderUpdated.Status;
        userOrder.ShippingAddress = orderUpdated.ShippingAddress;
        userOrder.OrderDate = DateTime.UtcNow;

        // Update or add items
        foreach (var updatedItem in orderUpdated.Items)
        {
            var existingItem = userOrder.Items
                .FirstOrDefault(i => i.ProductId == updatedItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = updatedItem.Quantity;
            }
            else
            {
                userOrder.Items.Add(
                    new OrderItem
                    {
                        ProductId = updatedItem.ProductId,
                        Quantity = updatedItem.Quantity,
                        OrderId = orderId
                    });
            }
        }

        // Remove items that are no longer present in the updated order
        var updatedProductIds = orderUpdated.Items
            .Select(i => i.ProductId)
            .ToHashSet();

        var itemsToRemove = userOrder.Items
            .Where(i => !updatedProductIds.Contains(i.ProductId))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            userOrder.Items.Remove(item);
        }

        _dbContext.Orders.Update(userOrder);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CancelOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
    }

}
