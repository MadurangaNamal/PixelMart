using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Identity;
using PixelMart.API.Models.Stock;
using PixelMart.API.Repositories;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/item-stocks")]
public class StocksController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IStocksRepository _stocksRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public StocksController(IPixelMartRepository pixelMartRepository, IStocksRepository stocksRepository, IProductsRepository productsRepository,
        IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _stocksRepository = stocksRepository ?? throw new ArgumentNullException(nameof(stocksRepository));
        _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpGet]
    [OutputCache]
    public async Task<ActionResult<IEnumerable<StockItemDto>>> GetAllStocks(CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo("GET /api/item-stocks CALLED TO RETRIEVE ALL STOCKS");

        var stocks = await _stocksRepository.GetAllItemStocksAsync(cancellationToken);
        var stocksResponse = _mapper.Map<IEnumerable<StockItemDto>>(stocks);

        return Ok(stocksResponse);
    }

    [HttpGet("{productId}", Name = "GetStockForProduct")]
    [OutputCache]
    public async Task<ActionResult<StockItemDto>> GetProductStock(Guid productId, CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"GET /api/item-stocks/{productId} CALLED TO RETRIEVE STOCKS FOR A PRODUCT");

        if (!await _stocksRepository.StockExistsAsync(productId, cancellationToken))
            return NotFound();

        var productStocksFromRepo = await _stocksRepository.GetItemStockAsync(productId, cancellationToken);
        var stockResponse = _mapper.Map<StockItemDto>(productStocksFromRepo);

        return Ok(stockResponse);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost("{productId}")]
    public async Task<IActionResult> AddProductStock(Guid productId, StockManipulationDto productStock, CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"POST /api/item-stocks/{productId} CALLED TO ADD STOCKS FOR A PRODUCT");

        if (!await _productsRepository.ProductExistsAsync(productId, cancellationToken))
            return NotFound();

        var stockEntity = _mapper.Map<Stock>(productStock);

        await _stocksRepository.AddItemStockAsync(productId, stockEntity, cancellationToken);
        await _pixelMartRepository.SaveAsync(cancellationToken);

        var productStockToReturn = _mapper.Map<StockItemDto>(stockEntity);

        return CreatedAtRoute("GetStockForProduct", new { productId = stockEntity.ProductId }, productStockToReturn);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductStock(Guid productId, StockManipulationDto productStock, CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"PUT /api/item-stocks/{productId} CALLED TO UPDATE STOCKS FOR A PRODUCT");

        if (!await _productsRepository.ProductExistsAsync(productId, cancellationToken))
            return NotFound();

        var productStockFromRepo = await _stocksRepository.GetItemStockAsync(productId, cancellationToken);

        if (productStockFromRepo is null)
        {
            var stockToAdd = _mapper.Map<Stock>(productStock);

            await _stocksRepository.AddItemStockAsync(productId, stockToAdd, cancellationToken);
            await _pixelMartRepository.SaveAsync(cancellationToken);

            var stockItemToReturn = _mapper.Map<StockItemDto>(stockToAdd);

            return CreatedAtRoute("GetStockForProduct", new { productId = stockItemToReturn.ProductId }, stockItemToReturn);
        }

        _mapper.Map(productStock, productStockFromRepo);
        await _stocksRepository.UpdateItemStockAsync(productId, productStockFromRepo, cancellationToken);

        return NoContent();
    }
}
