using Microsoft.Extensions.Caching.Memory;
using PixelMart.API.Models.Product;

namespace PixelMart.API.Services.Impl;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Product_";
    private TimeSpan DefaultExpiry = TimeSpan.FromMinutes(60);

    public CacheService(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public void SetProductDto(Guid productId, ProductDto productDto, TimeSpan? expiry = null)
    {
        _cache.Set($"{CacheKeyPrefix}{productId}", productDto, expiry ?? DefaultExpiry);
    }

    public ProductDto? GetProductDto(Guid productId)
    {
        return _cache.TryGetValue<ProductDto?>($"{CacheKeyPrefix}{productId}", out ProductDto? productDto) ? productDto : null;
    }

    public void SetAllProductDtos(Dictionary<Guid, ProductDto>? products, TimeSpan? expiry = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(products));

        foreach (var product in products!)
        {
            SetProductDto(product.Key, product.Value, expiry ?? DefaultExpiry);
        }
    }
}
