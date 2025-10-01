using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Entities;

public class Category
{
    public Category()
    {
    }

    public Category(string name)
    {
        Name = name;
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
