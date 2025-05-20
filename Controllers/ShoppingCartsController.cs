using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Identity;
using PixelMart.API.Models.ShoppingCart;
using PixelMart.API.Repositories;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shopping-carts")]
public class ShoppingCartsController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public ShoppingCartsController(IPixelMartRepository pixelMartRepository, IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper;
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingCartDto>>> GetAllCartDetails()
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts CALLED TO RETRIEVE ALL SHOPPING CARTS");

        var shoppingCarts = await _pixelMartRepository.GetAllCartDetailsAsync();

        return Ok(_mapper.Map<IEnumerable<ShoppingCartDto>>(shoppingCarts));
    }

    [HttpGet("user-cart", Name = "GetCartForUser")]
    public async Task<ActionResult<ShoppingCartDto>> GetCartDetails()
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts/user-cart CALLED TO RETRIEVE SHOPPING CART DETAILS FOR USER");

        var userId = getLoggedUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID.");

        var shoppingCart = await _pixelMartRepository.GetCartDetailsForUserAsync(userId);

        return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
    }

    [HttpPost]
    public async Task<IActionResult> AddToShoppingCart(ShoppingCartManipulationDto shoppingCart)
    {
        _requestLogHelper.LogInfo($"POST /api/shopping-carts CALLED TO ADD SHOPPING CART FOR A USER");

        var userId = getLoggedUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID.");

        var cartEntity = _mapper.Map<ShoppingCart>(shoppingCart);

        await _pixelMartRepository.AddShoppingCartAsync(userId, cartEntity);
        await _pixelMartRepository.SaveAsync();

        var cartToReturn = _mapper.Map<ShoppingCartDto>(cartEntity);

        return CreatedAtRoute("GetCartForUser", cartToReturn);
    }

    private Guid getLoggedUserId()
    {
        var userId = _requestLogHelper.GetUserID();

        if (userId == Guid.Empty)
            _requestLogHelper.LogError(null!, "Invalid user ID");

        return userId;
    }
}
