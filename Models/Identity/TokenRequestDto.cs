using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Identity;

public class TokenRequestDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}
