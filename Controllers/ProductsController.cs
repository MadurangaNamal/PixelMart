using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Helpers.ResourceParameters;
using PixelMart.API.Models.Identity;
using PixelMart.API.Models.Product;
using PixelMart.API.Repositories;
using PixelMart.API.Services;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories/{categoryId}/products")]
public class ProductsController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IStocksRepository _stocksRepository;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly RequestLogHelper _requestLogHelper;
    private readonly ICacheService _cacheService;

    public ProductsController(IPixelMartRepository pixelMartRepository,
        IProductsRepository productsRepository,
        ICategoriesRepository categoriesRepository,
        IStocksRepository stocksRepository,
        IMapper mapper,
        IPropertyMappingService propertyMappingService,
        RequestLogHelper requestLogHelper,
        ICacheService cacheService)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
        _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
        _stocksRepository = stocksRepository ?? throw new ArgumentNullException(nameof(stocksRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    [HttpGet(Name = "GetProductsForCategory")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
        Guid categoryId,
        [FromQuery] ProductsResourceParameters productsResourceParameters)
    {
        _requestLogHelper.LogInfo($"GET /api/categories/{categoryId}/products CALLED TO RETRIEVE PRODUCTS FOR A CATEGORY");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        // Validate the orderBy parameter
        if (!_propertyMappingService.ValidMappingExistsFor<ProductDto, Product>(productsResourceParameters.OrderBy))
            return BadRequest();

        var productsFromRepo = await _productsRepository.GetProductsAsync(categoryId, productsResourceParameters);

        foreach (Product product in productsFromRepo)
        {
            product.Stock = _stocksRepository.GetItemStockAsync(product.Id).Result;
        }

        var productsResponse = _mapper.Map<IEnumerable<ProductDto>>(productsFromRepo);

        return Ok(productsResponse);
    }

    [HttpGet("{productId}", Name = "GetProductForCategory")]
    public async Task<ActionResult<ProductDto>> GetProductForCategory(Guid categoryId, Guid productId)
    {
        _requestLogHelper.LogInfo($"GET /api/categories/{categoryId}/products/{productId} CALLED TO RETRIEVE SINGLE PRODUCT");

        var cacheKey = _cacheService.GetCategoryProductKey(categoryId, productId);

        if (_cacheService.GetProductDto(cacheKey) is { } cachedProductDto)
        {
            _requestLogHelper.LogInfo($"Cache HIT for Product {productId} in Category {categoryId}");

            return Ok(cachedProductDto);
        }

        var categoryExistsTask = _categoriesRepository.CategoryExistsAsync(categoryId);
        var productTask = _productsRepository.GetproductAsync(categoryId, productId);

        await Task.WhenAll(categoryExistsTask, productTask);

        if (!categoryExistsTask.Result)
            return NotFound();

        var productFromRepo = productTask.Result;

        if (productFromRepo == null)
            return NotFound();

        productFromRepo.Stock = await _stocksRepository.GetItemStockAsync(productFromRepo.Id);

        var productDto = _mapper.Map<ProductDto>(productFromRepo);
        _cacheService.SetProductDto(cacheKey, productDto);
        _requestLogHelper.LogInfo($"Cache MISS for Product {productId} in Category {categoryId}");

        return Ok(productDto);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost(Name = "CreateProductForCategory")]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, ProductForCreationDto product)
    {
        _requestLogHelper.LogInfo($"POST /api/categories/{categoryId}/products CALLED TO ADD A NEW PRODUCT");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var productEntity = _mapper.Map<Product>(product);

        await _productsRepository.AddProductAsync(categoryId, productEntity);
        await _pixelMartRepository.SaveAsync();

        var productToReturn = _mapper.Map<ProductDto>(productEntity);

        return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, ProductForUpdateDto product)
    {
        _requestLogHelper.LogInfo($"PUT /api/categories/{categoryId}/products/{productId} CALLED TO UPDATE A PRODUCT");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var productFromRepo = await _productsRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            var productToAdd = _mapper.Map<Product>(product);
            productToAdd.Id = productId;

            await _productsRepository.AddProductAsync(categoryId, productToAdd);
            await _pixelMartRepository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productToAdd);

            return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
        }

        _mapper.Map(product, productFromRepo); // apply new values

        await _productsRepository.UpdateProductAsync(productFromRepo);

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

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var productFromRepo = await _productsRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
        {
            var productDto = new ProductForUpdateDto();

            patchDocument.ApplyTo(productDto, ModelState);

            if (!TryValidateModel(productDto))
                return ValidationProblem(ModelState);

            var productToAdd = _mapper.Map<Product>(productDto);
            productToAdd.Id = productId;

            await _productsRepository.AddProductAsync(categoryId, productToAdd);
            await _pixelMartRepository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productToAdd);

            return CreatedAtRoute("GetProductForCategory", new { categoryId, productId = productToReturn.Id }, productToReturn);
        }

        var productToPatch = _mapper.Map<ProductForUpdateDto>(productFromRepo);

        patchDocument.ApplyTo(productToPatch, ModelState);

        if (!TryValidateModel(productToPatch))
            return ValidationProblem(ModelState);

        _mapper.Map(productToPatch, productFromRepo);

        await _productsRepository.UpdateProductAsync(productFromRepo);

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId)
    {
        _requestLogHelper.LogInfo($"DELETE /api/categories/{categoryId}/products/{productId} CALLED TO DELETE A PRODUCT");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var productFromRepo = await _productsRepository.GetproductAsync(categoryId, productId);

        if (productFromRepo == null)
            return NotFound();

        await _productsRepository.DeleteProductAsync(productFromRepo);

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
