using Microsoft.AspNetCore.Identity;

namespace PixelMart.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    //one-to-one relationship with ShoppingCart
    public ShoppingCart ShoppingCart { get; set; } = new ShoppingCart();

    //one-to-many relationship with Orders
    public ICollection<Order> Orders { get; set; } = [];
}
