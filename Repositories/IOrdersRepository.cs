using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderForUserAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
    Task CreateOrderAsync(Guid userId, Order order, CancellationToken cancellationToken = default);
    Task UpdateOrderAsync(Guid userId, Guid orderId, Order order, CancellationToken cancellationToken = default);
    Task CancelOrderAsync(Order order, CancellationToken cancellationToken = default);
}
