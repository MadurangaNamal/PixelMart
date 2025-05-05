using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Stock;

public class StockManipulationDto
{
    [Required]
    public int Quantity { get; set; }

    public int Threshold { get; set; }
}
