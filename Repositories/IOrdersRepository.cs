using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId);
    Task<Order?> GetOrderForUserAsync(Guid userId, Guid orderId);
    Task CreateOrderAsync(Guid userId, Order order);
    Task UpdateOrderAsync(Guid userId, Guid orderId, Order order);
    Task CancelOrderAsync(Order order);
}
