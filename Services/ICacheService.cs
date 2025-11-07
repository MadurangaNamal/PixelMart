using PixelMart.API.Models.Product;

namespace PixelMart.API.Services;

public interface ICacheService
{
    void SetProductDto(string key, ProductDto productDto, TimeSpan? expiry = null);
    ProductDto? GetProductDto(string key);
    string GetCategoryProductKey(Guid categoryId, Guid productId);
}
