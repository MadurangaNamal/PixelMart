using Microsoft.AspNetCore.Identity;

namespace PixelMart.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ShoppingCart ShoppingCart { get; set; } = new ShoppingCart();
    public ICollection<Order> Orders { get; set; } = [];
}
