using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class ShoppingCart
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<CartItem> Items { get; set; } = [];

    // Foreign key to ApplicationUser
    [Required]
    public string UserId { get; set; } = default!;

    // Navigation property to ApplicationUser
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = default!;

}
