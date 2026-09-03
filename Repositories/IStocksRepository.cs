using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IStocksRepository
{
    Task<IEnumerable<Stock>> GetAllItemStocksAsync(CancellationToken cancellationToken = default);
    Task<Stock?> GetItemStockAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddItemStockAsync(Guid productId, Stock stock, CancellationToken cancellationToken = default);
    Task UpdateItemStockAsync(Guid productId, Stock itemStock, CancellationToken cancellationToken = default);
    Task<bool> StockExistsAsync(Guid productId, CancellationToken cancellationToken = default);
}
