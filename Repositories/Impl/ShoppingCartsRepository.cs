using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;

namespace PixelMart.API.Repositories.Impl;

public class ShoppingCartsRepository : IShoppingCartsRepository
{
    private readonly PixelMartDbContext _dbContext;

    public ShoppingCartsRepository(PixelMartDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingCart?> GetCartDetailsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return await _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(sc => sc.UserId == userId.ToString(), cancellationToken);
    }

    public async Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(shoppingCart);

        shoppingCart.UserId = userId.ToString();
        await _dbContext.ShoppingCarts.AddAsync(shoppingCart, cancellationToken);
    }

    public async Task UpdateShoppingCartAsync(Guid userId, ShoppingCart updatedShoppingCart,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(updatedShoppingCart);

        var shoppingCart = await _dbContext.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.ToString(), cancellationToken);

        if (shoppingCart == null)
            throw new InvalidOperationException("Shopping cart not found.");

        foreach (var updatedItem in updatedShoppingCart.Items)
        {
            var existingItem = shoppingCart.Items
                .FirstOrDefault(i => i.ProductId == updatedItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = updatedItem.Quantity;
            }
            else
            {
                shoppingCart.Items.Add(new CartItem
                {
                    ProductId = updatedItem.ProductId,
                    Quantity = updatedItem.Quantity,
                    ShoppingCartId = shoppingCart.Id
                });
            }
        }

        _dbContext.ShoppingCarts.Update(shoppingCart);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cart);

        _dbContext.ShoppingCarts.Remove(cart);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
