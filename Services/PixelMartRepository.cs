using PixelMart.API.Entities;

namespace PixelMart.API.Services;

public class PixelMartRepository : IPixelMartRepository
{
    public void AddCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public void AddProduct(Guid categoryId, Product product)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public void DeleteProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync(IEnumerable<Guid> categoryIds)
    {
        throw new NotImplementedException();
    }

    public Task<Category> GetCategoryAsync(Guid categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<Product> GetproductAsync(Guid categoryId, Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAsync()
    {
        throw new NotImplementedException();
    }

    public void UpdateCategory(Category category)
    {
        throw new NotImplementedException();
    }

    public void UpdateProduct(Product product)
    {
        throw new NotImplementedException();
    }
}
