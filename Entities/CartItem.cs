using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class CartItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    // Foreign keys
    public Guid ShoppingCartId { get; set; }
    public Guid ProductId { get; set; }

    // Navigation properties
    [ForeignKey("ShoppingCartId")]
    public ShoppingCart ShoppingCart { get; set; } = null!;

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}
