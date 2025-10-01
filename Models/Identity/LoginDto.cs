using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Identity;

public class LoginDto
{
    [Required]
    public string EmailAddress { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
