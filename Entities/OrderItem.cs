using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    public Guid OrderId { get; set; }

    [JsonIgnore]
    [ForeignKey("OrderId")]
    public Order? Order { get; set; } = null!;

    public Guid ProductId { get; set; }

    [JsonIgnore]
    [ForeignKey("ProductId")]
    public Product? Product { get; set; } = null!;
}
