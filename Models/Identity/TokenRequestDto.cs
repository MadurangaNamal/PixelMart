using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Identity;

public class TokenRequestDto
{
    [Required]
    public required string Token { get; set; }

    [Required]
    public required string RefreshToken { get; set; }
}
