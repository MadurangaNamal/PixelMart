namespace PixelMart.API.Models.Identity;

public class AuthResultDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
