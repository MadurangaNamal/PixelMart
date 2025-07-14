using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Order;

public class OrderCreationDto : OrderManipulationDto
{
    [Required]
    public override required ICollection<OrderItemDto> Items { get; set; }
}
