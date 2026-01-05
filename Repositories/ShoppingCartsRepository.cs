using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public class ShoppingCartsRepository : IShoppingCartsRepository
{
    private readonly PixelMartDbContext _dbContext;

    public ShoppingCartsRepository(PixelMartDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync()
    {
        return await _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ShoppingCart?> GetCartDetailsForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return await _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(sc => sc.UserId == userId.ToString());
    }

    public async Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(shoppingCart);

        shoppingCart.UserId = userId.ToString();
        await _dbContext.ShoppingCarts.AddAsync(shoppingCart);
    }

    public async Task UpdateShoppingCartAsync(Guid userId, ShoppingCart updatedShoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(updatedShoppingCart);

        var shoppingCart = await _dbContext.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.ToString());

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
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteShoppingCartAsync(ShoppingCart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        _dbContext.ShoppingCarts.Remove(cart);
        await _dbContext.SaveChangesAsync();
    }

}
