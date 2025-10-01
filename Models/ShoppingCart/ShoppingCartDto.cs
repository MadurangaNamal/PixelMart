using PixelMart.API.Entities;

namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = [];

    public string UserId { get; set; } = default!;

}
