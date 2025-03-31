using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class Stock
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int Threshold { get; set; } = 10;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Foreign key (1:1 with Product)
    public Guid ProductId { get; set; }

    // Navigation property
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public bool IsLowStock => Quantity <= Threshold;
}
