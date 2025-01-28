using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Entities;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; } = [];

    public Category(string name)
    {
        Name = name;
    }
}
