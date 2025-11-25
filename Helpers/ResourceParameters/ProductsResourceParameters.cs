namespace PixelMart.API.Helpers.ResourceParameters;

/// <summary>
/// Encapsulates resource parameters for querying and paginating product data.
/// Supports search, paging, sorting, and field selection for API requests.
/// </summary>
public class ProductsResourceParameters
{
    const int maxPageSize = 20;
    private int _pageSize = 10;

    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
    }
    public string OrderBy { get; set; } = "Name";
    public string? Fields { get; set; }
}
