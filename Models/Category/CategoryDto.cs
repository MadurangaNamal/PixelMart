using PixelMart.API.Models.Product;

namespace PixelMart.API.Models.Category;

public class CategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();

}
