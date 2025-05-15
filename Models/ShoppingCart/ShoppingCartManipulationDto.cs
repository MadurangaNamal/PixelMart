using PixelMart.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.ShoppingCart;

public class ShoppingCartManipulationDto
{
    [Required]
    public virtual ICollection<CartItem> Items { get; set; }

}
