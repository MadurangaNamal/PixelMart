using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class CartItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    public Guid ShoppingCartId { get; set; }

    [ForeignKey("ShoppingCartId")]
    public ShoppingCart ShoppingCart { get; set; } = null!;

    public Guid ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}
