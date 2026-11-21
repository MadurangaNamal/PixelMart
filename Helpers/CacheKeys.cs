namespace PixelMart.API.Helpers;

public static class CacheKeys
{
    public static string Product(Guid categoryId, Guid productId)
        => $"Cat_{categoryId}_Prod_{productId}";
}
