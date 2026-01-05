using PixelMart.API.Entities;

namespace PixelMart.API.Repositories;

public interface ICategoriesRepository
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryAsync(Guid categoryId);
    Task AddCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task<bool> CategoryExistsAsync(Guid categoryId);
}
