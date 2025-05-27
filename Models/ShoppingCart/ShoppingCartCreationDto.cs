using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartCreationDto : ShoppingCartManipulationDto
{
    [Required]
    public override ICollection<CartItemDto> Items { get; set; }
}
