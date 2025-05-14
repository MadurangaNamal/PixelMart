using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models.Product;

public class ProductForUpdateDto : ProductForManipulationDto
{
    [Required(ErrorMessage = "You should fill out a brand")]
    public override string Brand
    {
        get => base.Brand;
        set => base.Brand = value;
    }

    [Required(ErrorMessage = "You should fill out a price")]
    public override decimal Price
    {
        get => base.Price;
        set => base.Price = value;
    }

    [Required(ErrorMessage = "You should fill out a description")]
    public override string Description
    {
        get => base.Description;
        set => base.Description = value;
    }
}
