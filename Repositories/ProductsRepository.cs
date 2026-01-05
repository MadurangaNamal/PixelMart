using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Helpers.ResourceParameters;
using PixelMart.API.Models.Product;
using PixelMart.API.Services;

namespace PixelMart.API.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly PixelMartDbContext _dbContext;
    private readonly IPropertyMappingService _propertyMappingService;
    public ProductsRepository(PixelMartDbContext dbContext, IPropertyMappingService propertyMappingService)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
    }

    public async Task<Product?> GetproductAsync(Guid categoryId, Guid productId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(productId));
        }

        return await _dbContext.Products
            .Where(p => p.Id == productId && p.CategoryId == categoryId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        return await _dbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddProductAsync(Guid categoryId, Product product)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        ArgumentNullException.ThrowIfNull(product);

        product.CategoryId = categoryId;
        await _dbContext.Products.AddAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedList<Product>> GetProductsAsync(
        Guid categoryId,
        ProductsResourceParameters productsResourceParameters)
    {
        if (productsResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(productsResourceParameters));
        }

        var productsCollection = _dbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.SearchQuery))
        {
            var searchQuery = productsResourceParameters.SearchQuery.Trim();
            productsCollection = productsCollection
                .Where(a => a.Name.Contains(searchQuery) || a.Brand.Contains(searchQuery));
        }

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.OrderBy))
        {
            // get property mapping dictionary
            var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<ProductDto, Product>();

            productsCollection = productsCollection
                .ApplySort(productsResourceParameters.OrderBy, authorPropertyMappingDictionary);
        }

        return await PagedList<Product>.CreateAsync(productsCollection,
             productsResourceParameters.PageNumber,
             productsResourceParameters.PageSize);
    }

    public async Task<bool> ProductExistsAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _dbContext.Products.AsNoTracking().AnyAsync(p => p.Id == productId);
    }

}
