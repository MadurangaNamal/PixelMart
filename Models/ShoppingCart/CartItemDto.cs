using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.ShoppingCart;

public class CartItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}
