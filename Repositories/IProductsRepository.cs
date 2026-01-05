using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Helpers.ResourceParameters;

namespace PixelMart.API.Repositories;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId);
    Task<Product?> GetproductAsync(Guid categoryId, Guid productId);
    Task AddProductAsync(Guid categoryId, Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductsResourceParameters productsResourceParameters);
    Task<bool> ProductExistsAsync(Guid productId);
}
