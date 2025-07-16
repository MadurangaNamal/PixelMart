using PixelMart.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Order;

public class OrderCreationDto : OrderManipulationDto
{
    [Required]
    public override required ICollection<OrderItem> Items { get; set; }
}
