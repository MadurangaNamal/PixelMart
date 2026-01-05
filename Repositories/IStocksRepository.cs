using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IStocksRepository
{
    Task<IEnumerable<Stock>> GetAllItemStocksAsync();
    Task<Stock?> GetItemStockAsync(Guid productId);
    Task AddItemStockAsync(Guid productId, Stock stock);
    Task UpdateItemStockAsync(Guid productId, Stock itemStock);
    Task<bool> StockExistsAsync(Guid productId);

}
