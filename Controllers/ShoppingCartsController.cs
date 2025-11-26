using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingCartDto>>> GetAllCartDetails()
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts CALLED TO RETRIEVE ALL SHOPPING CARTS");

        var shoppingCarts = await _pixelMartRepository.GetAllCartDetailsAsync();
        var shoppingCartsResponse = _mapper.Map<IEnumerable<ShoppingCartDto>>(shoppingCarts);

        return Ok(shoppingCartsResponse);
    }

    [HttpGet("user-cart", Name = "GetCartForUser")]
    public async Task<ActionResult<ShoppingCartDto>> GetCartDetails()
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts/user-cart CALLED TO RETRIEVE SHOPPING CART DETAILS FOR USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var shoppingCart = await _pixelMartRepository.GetCartDetailsForUserAsync(userId);
        var shoppingCartResponse = _mapper.Map<ShoppingCartDto>(shoppingCart);

        return Ok(shoppingCartResponse);
    }

    [HttpPost("user-cart")]
    public async Task<IActionResult> AddToShoppingCart(ShoppingCartCreationDto shoppingCartCreationDto)
    {
        _requestLogHelper.LogInfo($"POST /api/shopping-carts/user-cart CALLED TO ADD SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartEntity = _mapper.Map<ShoppingCart>(shoppingCartCreationDto);

        await _pixelMartRepository.AddShoppingCartAsync(userId, cartEntity);
        await _pixelMartRepository.SaveAsync();

        var cartToReturn = _mapper.Map<ShoppingCartDto>(cartEntity);

        return CreatedAtRoute("GetCartForUser", cartToReturn);
    }

    [HttpPut("user-cart")]
    public async Task<IActionResult> UpdateShoppingCart(ShoppingCartManipulationDto shoppingCartUpdateDto)
    {
        _requestLogHelper.LogInfo($"PUT /api/shopping-carts/user-cart CALLED TO UPDTAE THE SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartFromRepo = await _pixelMartRepository.GetCartDetailsForUserAsync(userId);

        if (cartFromRepo == null)
        {
            var CartToAdd = _mapper.Map<ShoppingCart>(shoppingCartUpdateDto);

            await _pixelMartRepository.AddShoppingCartAsync(userId, CartToAdd);
            await _pixelMartRepository.SaveAsync();

            return CreatedAtRoute("GetCartForUser", CartToAdd);
        }

        _mapper.Map(shoppingCartUpdateDto, cartFromRepo);
        await _pixelMartRepository.UpdateShoppingCartAsync(userId, cartFromRepo);

        return NoContent();
    }

    [HttpPatch("user-cart")]
    public async Task<IActionResult> PartiallyUpdateShoppingCart(JsonPatchDocument<ShoppingCartUpdateDto> patchDocument)
    {
        _requestLogHelper.LogInfo($"PATCH /api/shopping-carts/user-cart CALLED TO PARTIALLY UPDTAE THE SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartFromRepo = await _pixelMartRepository.GetCartDetailsForUserAsync(userId);

        if (cartFromRepo == null)
        {
            var shoppingCartDto = new ShoppingCartUpdateDto();
            
            patchDocument.ApplyTo(shoppingCartDto, ModelState);

            if (!TryValidateModel(shoppingCartDto))
                return ValidationProblem(ModelState);

            var cartToAdd = _mapper.Map<ShoppingCart>(shoppingCartDto);

            await _pixelMartRepository.AddShoppingCartAsync(userId, cartToAdd);
            await _pixelMartRepository.SaveAsync();

            var cartToReturn = _mapper.Map<ShoppingCartDto>(cartToAdd);

            return CreatedAtRoute("GetCartForUser", cartToReturn);
        }

        var shoppingCartToPatch = _mapper.Map<ShoppingCartUpdateDto>(cartFromRepo);

        patchDocument.ApplyTo(shoppingCartToPatch, ModelState);

        if (!TryValidateModel(shoppingCartToPatch))
            return ValidationProblem(ModelState);

        _mapper.Map(shoppingCartToPatch, cartFromRepo);
        await _pixelMartRepository.UpdateShoppingCartAsync(userId, cartFromRepo);

        return NoContent();
    }

    [HttpDelete("user-cart")]
    public async Task<IActionResult> DeleteUserShoppingCart()
    {
        _requestLogHelper.LogInfo($"DELETE /api/shopping-carts/user-cart CALLED TO DELETE SHOPPING CART FOR USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartForUserFromRepo = await _pixelMartRepository.GetCartDetailsForUserAsync(userId);

        if (cartForUserFromRepo is null)
            return NotFound();

        await _pixelMartRepository.DeleteShoppingCartAsync(cartForUserFromRepo);

        return NoContent();
    }

    private Guid getCurrentUserId()
    {
        var userId = _requestLogHelper.GetUserId();

        if (userId == Guid.Empty)
            _requestLogHelper.LogError(null!, "Invalid user ID");

        return userId;
    }
}
