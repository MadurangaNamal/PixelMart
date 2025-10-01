using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public string Token { get; set; } = default!;
    public string JwtId { get; set; } = default!;
    public bool IsRevoked { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateExpire { get; set; }
    public string UserId { get; set; } = default!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;

}
