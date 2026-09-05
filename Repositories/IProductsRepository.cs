using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Helpers.ResourceParameters;

namespace PixelMart.API.Repositories;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductsResourceParameters productsResourceParameters,
        CancellationToken cancellationToken = default);
    Task<PagedList<Product>> GetAllProductsAsync(ProductsResourceParameters productsResourceParameters,
        CancellationToken cancellationToken = default);
    Task<Product?> GetproductAsync(Guid categoryId, Guid productId, CancellationToken cancellationToken = default);
    Task AddProductAsync(Guid categoryId, Product product, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> ProductExistsAsync(Guid productId, CancellationToken cancellationToken = default);
}
