namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartUpdateDto : ShoppingCartManipulationDto
{
    public override ICollection<CartItemDto> Items { get; set; } = [];
}
