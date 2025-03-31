using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class Product
{
    public Product()
    {
    }

    public Product(string name)
    {
        Name = name;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(1500)]
    public string? Description { get; set; }

    public Guid CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public Stock? Stock { get; set; }
}
