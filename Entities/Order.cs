using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class Order
{
    public Order()
    {
        OrderNumber = GenerateOrderNumber();
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public string OrderNumber { get; private set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = [];

    // Foreign key to ApplicationUser
    [Required]
    public string UserId { get; set; }

    // Navigation property to ApplicationUser
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    private static string GenerateOrderNumber()
    {
        return "ORD-" + DateTime.UtcNow
            .ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid()
            .ToString().Substring(0, 4).ToUpper();
    }
}
