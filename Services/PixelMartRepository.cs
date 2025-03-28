using Microsoft.EntityFrameworkCore;
using PixelMart.API.DbContexts;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.ResourceParameters;

namespace PixelMart.API.Services;

public class PixelMartRepository : IPixelMartRepository
{
    private readonly PixelMartDbContext _context;

    public PixelMartRepository(PixelMartDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void AddCategory(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        category.Id = Guid.NewGuid();

        foreach (var product in category.Products)
        {
            product.Id = Guid.NewGuid();
        }

        _context.Categories.Add(category);
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

    public void DeleteProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _context.Products.Remove(product);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync(IEnumerable<Guid> categoryIds)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
#pragma warning restore CS8603 // Possible null reference return.

    }

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
        return await _context.Products.Where(p => p.Id == productId && p.CategoryId == categoryId).FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.

    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        return await _context.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }

    public void UpdateCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public void UpdateProduct(Product product)
    {
        throw new NotImplementedException();
    }

    //public async Task<PagedList<Category>> GetCategoriesAsync(
    //    CategoriesResourceParameters categoriesResourceParameters)
    //{
    //    ArgumentNullException.ThrowIfNull(categoriesResourceParameters);

    //    return 
    //}
}
