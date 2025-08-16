using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Identity;
using PixelMart.API.Models.Product;
using PixelMart.API.Repositories;
using PixelMart.API.ResourceParameters;
using PixelMart.API.Services;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories/{categoryId}/products")]
public class ProductsController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly RequestLogHelper _requestLogHelper;

    public ProductsController(IPixelMartRepository pixelMartRepository,
        IMapper mapper,
        IPropertyMappingService propertyMappingService,
        RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _propertyMappingService = propertyMappingService;
        _requestLogHelper = requestLogHelper;
    }

    [HttpGet(Name = "GetProductsForCategory")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(Guid categoryId,
        [FromQuery] ProductsResourceParameters productsResourceParameters)
    {
        _requestLogHelper.LogInfo($"GET /api/categories/{categoryId}/products CALLED TO RETRIEVE PRODUCTS FOR A CATEGORY");

        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        if (!_propertyMappingService
            .ValidMappingExistsFor<ProductDto, Product>(
                productsResourceParameters.OrderBy))
        {
            return BadRequest();
        }

        var productsFromRepo = await _pixelMartRepository.GetProductsAsync(categoryId, productsResourceParameters);

        foreach (Product product in productsFromRepo)
        {
            product.Stock = _pixelMartRepository.GetItemStockAsync(product.Id).Result;
        }

        return Ok(_mapper.Map<IEnumerable<ProductDto>>(productsFromRepo));
    }

    [HttpGet("{productId}", Name = "GetProductForCategory")]
    public async Task<ActionResult<ProductDto>> GetProductForCategory(Guid categoryId, Guid productId)
    {
        _requestLogHelper.LogInfo($"GET /api/categories/{categoryId}/products/{productId} CALLED TO RETRIEVE SINGLE PRODUCT");

        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            return NotFound();
        }

        productFromRepo.Stock = _pixelMartRepository.GetItemStockAsync(productFromRepo.Id).Result;
        return Ok(_mapper.Map<ProductDto>(productFromRepo));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost(Name = "CreateProductForCategory")]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, ProductForCreationDto product)
    {
        _requestLogHelper.LogInfo($"POST /api/categories/{categoryId}/products CALLED TO ADD A NEW PRODUCT");

        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productEntity = _mapper.Map<Product>(product);
        await _pixelMartRepository.AddProductAsync(categoryId, productEntity);
        await _pixelMartRepository.SaveAsync();

        var productToReturn = _mapper.Map<ProductDto>(productEntity);

        return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, ProductForUpdateDto product)
    {
        _requestLogHelper.LogInfo($"PUT /api/categories/{categoryId}/products/{productId} CALLED TO UPDATE A PRODUCT");

        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            var productToAdd = _mapper.Map<Product>(product);
            productToAdd.Id = productId;
            await _pixelMartRepository.AddProductAsync(categoryId, productToAdd);
            await _pixelMartRepository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productToAdd);

            return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
        }

        _mapper.Map(product, productFromRepo); // apply the updated field values to the entity
        await _pixelMartRepository.UpdateProductAsync(productFromRepo);

        return NoContent();
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPatch("{productId}")]
    public async Task<IActionResult> PartiallyUpdateProductForCategory(
        Guid categoryId,
        Guid productId,
        JsonPatchDocument<ProductForUpdateDto> patchDocument)
    {
        _requestLogHelper.LogInfo($"PATCH /api/categories/{categoryId}/products/{productId} CALLED TO PARTIALLY UPDATE A PRODUCT");

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

            var productToAdd = _mapper.Map<Product>(productDto);
            productToAdd.Id = productId;

            await _pixelMartRepository.AddProductAsync(categoryId, productToAdd);
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

        await _pixelMartRepository.UpdateProductAsync(productFromRepo);

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId)
    {
        _requestLogHelper.LogInfo($"DELETE /api/categories/{categoryId}/products/{productId} CALLED TO DELETE A PRODUCT");

        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var productFromRepo = await _pixelMartRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
            return NotFound();

        await _pixelMartRepository.DeleteProductAsync(productFromRepo);
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
