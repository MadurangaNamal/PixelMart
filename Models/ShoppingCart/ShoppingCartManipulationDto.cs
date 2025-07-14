namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartManipulationDto
{
    public virtual ICollection<CartItemDto> Items { get; set; } = [];
}
