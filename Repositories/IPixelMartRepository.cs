using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.ResourceParameters;

namespace PixelMart.API.Repositories;

public interface IPixelMartRepository
{
    #region Product
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId);
    Task<Product> GetproductAsync(Guid categoryId, Guid productId);
    void AddProduct(Guid categoryId, Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductsResourceParameters productsResourceParameters);
    Task<bool> ProductExistsAsync(Guid productId);

    #endregion

    #region Category
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryAsync(Guid categoryId);
    void AddCategory(Category category);
    void DeleteCategory(Category category);
    void UpdateCategory(Category category);
    Task<bool> CategoryExistsAsync(Guid categoryId);

    #endregion

    #region Stock
    Task<IEnumerable<Stock>> GetAllItemStocksAsync();
    Task<Stock> GetItemStockAsync(Guid productId);
    void AddItemStock(Guid productId, Stock stock);
    void UpdateItemStock(Guid productId, Stock itemStock);
    Task<bool> StockExistsAsync(Guid productId);

    #endregion

    #region Common
    Task<bool> SaveAsync();

    #endregion
}
