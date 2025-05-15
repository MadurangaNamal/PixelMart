using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public StocksController(IPixelMartRepository pixelMartRepository, IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper;
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockItemDto>>> GetAllStocks()
    {
        _requestLogHelper.LogInfo("GET /api/item-stocks CALLED TO RETRIEVE ALL STOCKS");

        var stocks = await _pixelMartRepository.GetAllItemStocksAsync();

        return Ok(_mapper.Map<IEnumerable<StockItemDto>>(stocks));
    }

    [HttpGet("{productId}", Name = "GetStockForProduct")]
    public async Task<ActionResult<StockItemDto>> GetProductStock(Guid productId)
    {
        _requestLogHelper.LogInfo($"GET /api/item-stocks/{productId} CALLED TO RETRIEVE STOCKS FOR A PRODUCT");

        if (!await _pixelMartRepository.StockExistsAsync(productId))
        {
            return NotFound();
        }

        var productStocksFromRepo = await _pixelMartRepository.GetItemStockAsync(productId);
        return Ok(_mapper.Map<StockItemDto>(productStocksFromRepo));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost("{productId}")]
    public async Task<IActionResult> AddProductStock(Guid productId, StockManipulationDto productStock)
    {
        _requestLogHelper.LogInfo($"POST /api/item-stocks/{productId} CALLED TO ADD STOCKS FOR A PRODUCT");

        if (!await _pixelMartRepository.ProductExistsAsync(productId))
        {
            return NotFound();
        }

        var stockEntity = _mapper.Map<Stock>(productStock);
        await _pixelMartRepository.AddItemStockAsync(productId, stockEntity);
        await _pixelMartRepository.SaveAsync();

        var productStockToReturn = _mapper.Map<StockItemDto>(stockEntity);

        return CreatedAtRoute("GetStockForProduct", new { productId = stockEntity.ProductId }, productStockToReturn);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductStock(Guid productId, StockManipulationDto productStock)
    {
        _requestLogHelper.LogInfo($"PUT /api/item-stocks/{productId} CALLED TO UPDATE STOCKS FOR A PRODUCT");

        if (!await _pixelMartRepository.ProductExistsAsync(productId))
        {
            return NotFound();
        }

        var productStockFromRepo = await _pixelMartRepository.GetItemStockAsync(productId);

        if (productStockFromRepo is null)
        {
            var stockToAdd = _mapper.Map<Stock>(productStock);
            await _pixelMartRepository.AddItemStockAsync(productId, stockToAdd);
            await _pixelMartRepository.SaveAsync();

            var stockItemToReturn = _mapper.Map<StockItemDto>(stockToAdd);
            return CreatedAtRoute("GetStockForProduct", new { productId = stockItemToReturn.ProductId }, stockItemToReturn);
        }

        _mapper.Map(productStock, productStockFromRepo);
        await _pixelMartRepository.UpdateItemStockAsync(productId, productStockFromRepo);

        return NoContent();
    }
}
