using Microsoft.EntityFrameworkCore;
using PixelMart.API.Data;
using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly PixelMartDbContext _dbContext;

    public CategoriesRepository(PixelMartDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        // return await _context.Categories.Include(c => c.Products).ToListAsync(); // include product details if needed
        return await _dbContext.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        return await _dbContext.Categories
            .Include(c => c.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task AddCategoryAsync(Category category)
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

        await _dbContext.Categories.AddAsync(category);
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        return await _dbContext.Categories.AsNoTracking().AnyAsync(c => c.Id == categoryId);
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
    }

}
