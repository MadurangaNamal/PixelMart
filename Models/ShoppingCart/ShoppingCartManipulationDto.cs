using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartManipulationDto
{
    [Required]
    public ICollection<CartItemDto> Items { get; set; }
}
