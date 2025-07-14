using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Order;

public class OrderUpdateDto : OrderManipulationDto
{
    [Required]
    public override required string OrderStatus { get; set; }
}
