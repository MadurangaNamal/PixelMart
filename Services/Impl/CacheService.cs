using Microsoft.Extensions.Caching.Memory;

namespace PixelMart.API.Services.Impl;

public class CacheService : ICacheService
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(60);
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim[] _locks =
        Enumerable.Range(0, 32).Select(_ => new SemaphoreSlim(1, 1)).ToArray();  // Striped locks: bounded memory, no per-key semaphore leak

    public CacheService(IMemoryCache cache) => _cache = cache;

    public T? Get<T>(string key)
        => _cache.TryGetValue(key, out T? value) ? value : default;

    public void Set<T>(string key, T value, TimeSpan? expiry = null)
        => _cache.Set(key, value, expiry ?? DefaultExpiry);

    public void Remove(string key) => _cache.Remove(key);

    // 
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
            return cached;

        var gate = _locks[(int)((uint)key.GetHashCode() % (uint)_locks.Length)];
        await gate.WaitAsync(cancellationToken);

        try
        {
            // Re-check: another request may have populated it while we waited
            if (_cache.TryGetValue(key, out cached) && cached is not null)
                return cached;

            var value = await factory(cancellationToken);
            _cache.Set(key, value, expiry ?? DefaultExpiry);
            return value;
        }
        finally
        {
            gate.Release();
        }
    }
}
