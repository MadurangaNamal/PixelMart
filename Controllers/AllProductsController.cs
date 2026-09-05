using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Helpers.ResourceParameters;
using PixelMart.API.Models.Product;
using PixelMart.API.Repositories;
using PixelMart.API.Services;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class AllProductsController : ControllerBase
{
    private readonly IProductsRepository _productsRepository;
    private readonly IStocksRepository _stocksRepository;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly RequestLogHelper _requestLogHelper;
    private readonly ICacheService _cacheService;

    public AllProductsController(
        IProductsRepository productsRepository,
        IStocksRepository stocksRepository,
        IMapper mapper,
        IPropertyMappingService propertyMappingService,
        RequestLogHelper requestLogHelper,
        ICacheService cacheService)
    {
        _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
        _stocksRepository = stocksRepository ?? throw new ArgumentNullException(nameof(stocksRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    [HttpGet(Name = "GetAllProducts")]
    [OutputCache]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(
        [FromQuery] ProductsResourceParameters resourceParameters,
        CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo("GET /api/products CALLED TO RETRIEVE ALL PRODUCTS");

        if (!_propertyMappingService.ValidMappingExistsFor<ProductDto, Product>(resourceParameters.OrderBy))
            return BadRequest();

        var productsFromRepo = await _productsRepository.GetAllProductsAsync(resourceParameters, cancellationToken);

        foreach (Product product in productsFromRepo)
        {
            product.Stock = await _stocksRepository.GetItemStockAsync(product.Id, cancellationToken);
        }

        var productsResponse = _mapper.Map<IEnumerable<ProductDto>>(productsFromRepo);

        return Ok(productsResponse);
    }
}
