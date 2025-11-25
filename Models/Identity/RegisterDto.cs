using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Identity;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string EmailAddress { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    public string Role { get; set; } = UserRoles.Client;
}
