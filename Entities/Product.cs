using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelMart.API.Entities;

public class Product
{
    public Product(string name)
    {
        Name = name;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;

    public Guid CategoryId { get; set; }
}
