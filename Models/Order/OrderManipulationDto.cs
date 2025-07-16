using PixelMart.API.Entities;

namespace PixelMart.API.Models.Order;

public class OrderManipulationDto
{
    public virtual decimal TotalAmount { get; set; }
    public virtual required string OrderStatus { get; set; }
    public virtual required string ShippingAddress { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; } = [];
}
