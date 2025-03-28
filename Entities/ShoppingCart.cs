using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Entities;

public class ShoppingCart
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<CartItem> Items { get; set; } = [];

    public string UserId { get; set; } = null!;
}
