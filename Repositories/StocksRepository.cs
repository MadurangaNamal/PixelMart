using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public class StocksRepository : IStocksRepository
{
    private readonly PixelMartDbContext _dbContext;

    public StocksRepository(PixelMartDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Stock>> GetAllItemStocksAsync()
    {
        return await _dbContext.Stocks.AsNoTracking().ToListAsync();
    }

    public async Task<Stock?> GetItemStockAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _dbContext.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductId == productId);
    }

    public async Task AddItemStockAsync(Guid productId, Stock stock)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        ArgumentNullException.ThrowIfNull(stock);

        stock.ProductId = productId;
        await _dbContext.Stocks.AddAsync(stock);
    }

    public async Task UpdateItemStockAsync(Guid productId, Stock itemStock)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        ArgumentNullException.ThrowIfNull(itemStock);

        itemStock.ProductId = productId;

        _dbContext.Stocks.Update(itemStock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> StockExistsAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _dbContext.Stocks
            .AsNoTracking()
            .AnyAsync(s => s.ProductId == productId);
    }

}
