using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Models;
using PixelMart.API.Services;

namespace PixelMart.API.Controllers;

[ApiController]
[Route("api/categories/{categoryId}/products")]
public class ProductsController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IMapper _mapper;

    public ProductsController(IPixelMartRepository pixelMartRepository, IMapper mapper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(Name = "GetProductsForCategory")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsForCategory(Guid categoryId)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productsFromRepo = await _pixelMartRepository.GetProductsAsync(categoryId);
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(productsFromRepo));
    }


}
