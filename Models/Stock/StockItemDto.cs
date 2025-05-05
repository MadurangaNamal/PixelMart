namespace PixelMart.API.Models.Stock;

public class StockItemDto
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public int Threshold { get; set; }

    public DateTime LastUpdated { get; set; }

    public Guid ProductId { get; set; }

    public bool IsLowStock { get; set; }
}
