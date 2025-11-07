using Microsoft.Extensions.Caching.Memory;
using PixelMart.API.Models.Product;

namespace PixelMart.API.Services.Impl;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(60);

    public CacheService(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public void SetProductDto(string key, ProductDto productDto, TimeSpan? expiry = null)
    {
        _cache.Set(key, productDto, expiry ?? _defaultExpiry);
    }

    public ProductDto? GetProductDto(string key)
    {
        return _cache.TryGetValue<ProductDto?>(key, out ProductDto? productDto) ? productDto : null;
    }

    public string GetCategoryProductKey(Guid categoryId, Guid productId)
        => $"Cat_{categoryId}_Prod_{productId}";

}
