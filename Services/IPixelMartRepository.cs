using PixelMart.API.Entities;

namespace PixelMart.API.Services;

public interface IPixelMartRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId);
    Task<Product> GetproductAsync(Guid categoryId, Guid productId);
    void AddProduct(Guid categoryId, Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    Task<IEnumerable<Category>> GetCategoriesAsync();

    //Task<PagedList<Category>> GetCategoriesAsync(CategoriesResourceParameters categoriesResourceParameters);

    Task<Category> GetCategoryAsync(Guid categoryId);
    //Task<IEnumerable<Category>> GetCategoriesAsync(IEnumerable<Guid> categoryIds);
    void AddCategory(Category category);
    void DeleteCategory(Category category);
    void UpdateCategory(Category category);
    Task<bool> CategoryExistsAsync(Guid categoryId);
    Task<bool> SaveAsync();
}
