using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.ResourceParameters;

namespace PixelMart.API.Repositories;

public interface IPixelMartRepository
{
    #region Product
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId);
    Task<Product> GetproductAsync(Guid categoryId, Guid productId);
    Task AddProductAsync(Guid categoryId, Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductsResourceParameters productsResourceParameters);
    Task<bool> ProductExistsAsync(Guid productId);

    #endregion

    #region Category
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryAsync(Guid categoryId);
    Task AddCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task<bool> CategoryExistsAsync(Guid categoryId);

    #endregion

    #region Stock
    Task<IEnumerable<Stock>> GetAllItemStocksAsync();
    Task<Stock> GetItemStockAsync(Guid productId);
    Task AddItemStockAsync(Guid productId, Stock stock);
    Task UpdateItemStockAsync(Guid productId, Stock itemStock);
    Task<bool> StockExistsAsync(Guid productId);

    #endregion

    #region Shopping Cart
    Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync();
    Task<ShoppingCart> GetCartDetailsForUserAsync(Guid userId);
    Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart);
    Task UpdateShoppingCartAsync(Guid userId, ShoppingCart shoppingCart);
    Task DeleteShoppingCartAsync(ShoppingCart cart);
    #endregion

    #region Common
    Task<bool> SaveAsync();

    #endregion
}
