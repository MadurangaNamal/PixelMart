using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Identity;

public class RegisterDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    [Required]
    public string EmailAddress { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    [Required]
    public string Role { get; set; } = default!;
}
