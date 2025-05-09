namespace PixelMart.API.Models.Product;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0.00m;

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public bool IsLowStock { get; set; }
}
