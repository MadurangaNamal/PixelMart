using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
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

    [HttpGet("{productId}", Name = "GetProductForCategory")]
    public async Task<ActionResult<ProductDto>> GetProductForCategory(Guid categoryId, Guid productId)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ProductDto>(productFromRepo));
    }

    [HttpPost(Name = "CreateProductForCategory")]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, ProductForCreationDto product)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productEntity = _mapper.Map<Entities.Product>(product);

        _pixelMartRepository.AddProduct(categoryId, productEntity);

        await _pixelMartRepository.SaveAsync();

        var productToReturn = _mapper.Map<ProductDto>(productEntity);

        return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, ProductForUpdateDto product)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            var productToAdd = _mapper.Map<Entities.Product>(product);
            productToAdd.Id = productId;

            _pixelMartRepository.AddProduct(categoryId, productToAdd);
            await _pixelMartRepository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productToAdd);

            return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
        }

        _mapper.Map(product, productFromRepo); // apply the updated field values to the entity

        _pixelMartRepository.UpdateProduct(productFromRepo);

        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }

    [HttpPatch("{productId}")]
    public async Task<IActionResult> PartiallyUpdateProductForCategory(
        Guid categoryId,
        Guid productId,
        JsonPatchDocument<ProductForUpdateDto> patchDocument)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            var productDto = new ProductForUpdateDto();
            patchDocument.ApplyTo(productDto, ModelState);

            if (!TryValidateModel(productDto))
            {
                return ValidationProblem(ModelState);
            }

            var productToAdd = _mapper.Map<Entities.Product>(productDto);
            productToAdd.Id = productId;

            _pixelMartRepository.AddProduct(categoryId, productToAdd);
            await _pixelMartRepository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productToAdd);

            return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
        }

        var productToPatch = _mapper.Map<ProductForUpdateDto>(productFromRepo);

        patchDocument.ApplyTo(productToPatch, ModelState);

        if (!TryValidateModel(productToPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(productToPatch, productFromRepo);

        _pixelMartRepository.UpdateProduct(productFromRepo);

        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }
        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            return NotFound();
        }

        _pixelMartRepository.DeleteProduct(productFromRepo);
        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }

    public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
    {
        var options = HttpContext.RequestServices
            .GetRequiredService<IOptions<ApiBehaviorOptions>>();

        return (ActionResult)options.Value
            .InvalidModelStateResponseFactory(ControllerContext);
    }
}
