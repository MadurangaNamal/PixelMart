using Microsoft.AspNetCore.Identity;

namespace PixelMart.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    //one-to-one relationship with ShoppingCart
    public ShoppingCart ShoppingCart { get; set; }
}
