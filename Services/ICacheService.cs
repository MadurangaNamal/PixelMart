using PixelMart.API.Models.Product;

namespace PixelMart.API.Services;

public interface ICacheService
{
    void SetProductDto(Guid productId, ProductDto productDto, TimeSpan? expiry = null);
    ProductDto? GetProductDto(Guid productId);
    void SetAllProductDtos(Dictionary<Guid, ProductDto> products, TimeSpan? expiry = null);
}
