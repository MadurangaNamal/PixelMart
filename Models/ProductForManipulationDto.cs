using System.ComponentModel.DataAnnotations;

namespace PixelMart.API.Models;

public abstract class ProductForManipulationDto
{
    [Required(ErrorMessage = "You should fill out a name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250, ErrorMessage = "The brand shouldn't have more than 250 characters.")]
    public virtual string Brand { get; set; } = string.Empty;

    [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
    public virtual string Description { get; set; } = string.Empty;
}
