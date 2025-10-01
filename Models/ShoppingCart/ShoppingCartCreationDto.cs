using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartCreationDto : ShoppingCartManipulationDto
{
    [Required]
    public override required ICollection<CartItemDto> Items { get; set; }
}
