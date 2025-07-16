using PixelMart.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Order;

public class OrderUpdateDto : OrderManipulationDto
{
    [Required]
    public override required string OrderStatus { get; set; }

    [Required]
    public override required ICollection<OrderItem> Items { get; set; }
}
