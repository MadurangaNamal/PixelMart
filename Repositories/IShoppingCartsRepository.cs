using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IShoppingCartsRepository
{
    Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync();
    Task<ShoppingCart?> GetCartDetailsForUserAsync(Guid userId);
    Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart);
    Task UpdateShoppingCartAsync(Guid userId, ShoppingCart updatedShoppingCart);
    Task DeleteShoppingCartAsync(ShoppingCart cart);
}
