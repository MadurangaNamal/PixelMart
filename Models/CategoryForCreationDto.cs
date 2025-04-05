namespace PixelMart.API.Models;

public class CategoryForCreationDto
{
    public string Name { get; set; } = string.Empty;

    public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
}
