using Microsoft.EntityFrameworkCore;
using PixelMart.API.DbContexts;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models;
using PixelMart.API.ResourceParameters;
using PixelMart.API.Services;

namespace PixelMart.API.Repositories;

public class PixelMartRepository : IPixelMartRepository
{
    private readonly PixelMartDbContext _context;
    private readonly IPropertyMappingService _propertyMappingService;

    public PixelMartRepository(PixelMartDbContext context, IPropertyMappingService propertyMappingService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _propertyMappingService = propertyMappingService ??
            throw new ArgumentNullException(nameof(propertyMappingService));
    }

    #region Product
    public async Task<Product> GetproductAsync(Guid categoryId, Guid productId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(productId));
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Products
            .Where(p => p.Id == productId && p.CategoryId == categoryId)
            .FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.

    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public void AddProduct(Guid categoryId, Product product)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        ArgumentNullException.ThrowIfNull(product);

        product.CategoryId = categoryId;
        _context.Products.Add(product);
    }

    public void UpdateProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _context.Products.Update(product);
    }

    public void DeleteProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _context.Products.Remove(product);
    }

    public async Task<PagedList<Product>> GetProductsAsync(
        Guid categoryId,
        ProductsResourceParameters productsResourceParameters)
    {
        if (productsResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(productsResourceParameters));
        }

        var productsCollection = _context.Products as IQueryable<Product>;

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.SearchQuery))
        {
            var searchQuery = productsResourceParameters.SearchQuery.Trim();
            productsCollection = productsCollection.Where(a => a.Name.Contains(searchQuery) || a.Brand.Contains(searchQuery));
        }

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.OrderBy))
        {
            // get property mapping dictionary
            var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<ProductDto, Product>();

#pragma warning disable S1854 // Unused assignments should be removed
            productsCollection = productsCollection.ApplySort(productsResourceParameters.OrderBy, authorPropertyMappingDictionary);
#pragma warning restore S1854 // Unused assignments should be removed

        }

        return await PagedList<Product>.CreateAsync(productsCollection,
             productsResourceParameters.PageNumber,
             productsResourceParameters.PageSize);
    }

    #endregion

    #region Category
    public void AddCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        category.Id = Guid.NewGuid();

        if (category.Products.Any())
        {
            foreach (var product in category.Products)
            {
                product.Id = Guid.NewGuid();
            }
        }

        _context.Categories.Add(category);
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public void DeleteCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        _context.Categories.Remove(category);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.Include(c => c.Products).ToListAsync();
    }

    public async Task<Category> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
#pragma warning restore CS8603 // Possible null reference return.

    }

    public void UpdateCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        _context.Categories.Update(category);
    }

    #endregion

    #region Common
    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    #endregion

}
