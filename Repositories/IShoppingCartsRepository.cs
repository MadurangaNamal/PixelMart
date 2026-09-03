using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface IShoppingCartsRepository
{
    Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync(CancellationToken cancellationToken = default);
    Task<ShoppingCart?> GetCartDetailsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
    Task UpdateShoppingCartAsync(Guid userId, ShoppingCart updatedShoppingCart, CancellationToken cancellationToken = default);
    Task DeleteShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
}
