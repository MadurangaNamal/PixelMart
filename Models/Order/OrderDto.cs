using PixelMart.API.Entities;

namespace PixelMart.API.Models.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = [];
    public string UserId { get; set; } = null!;
}
