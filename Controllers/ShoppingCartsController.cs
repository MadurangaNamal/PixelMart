using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
    private readonly IShoppingCartsRepository _shoppingCartsRepository;
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public ShoppingCartsController(IPixelMartRepository pixelMartRepository, IShoppingCartsRepository shoppingCartsRepository,
        IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _shoppingCartsRepository = shoppingCartsRepository ?? throw new ArgumentNullException(nameof(shoppingCartsRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpGet]
    [OutputCache]
    public async Task<ActionResult<IEnumerable<ShoppingCartDto>>> GetAllCartDetails(CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts CALLED TO RETRIEVE ALL SHOPPING CARTS");

        var shoppingCarts = await _shoppingCartsRepository.GetAllCartDetailsAsync(cancellationToken);
        var shoppingCartsResponse = _mapper.Map<IEnumerable<ShoppingCartDto>>(shoppingCarts);

        return Ok(shoppingCartsResponse);
    }

    [HttpGet("user-cart", Name = "GetCartForUser")]
    [OutputCache]
    public async Task<ActionResult<ShoppingCartDto>> GetCartDetails(CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo("GET /api/shopping-carts/user-cart CALLED TO RETRIEVE SHOPPING CART DETAILS FOR USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var shoppingCart = await _shoppingCartsRepository.GetCartDetailsForUserAsync(userId, cancellationToken);
        var shoppingCartResponse = _mapper.Map<ShoppingCartDto>(shoppingCart);

        return Ok(shoppingCartResponse);
    }

    [HttpPost("user-cart")]
    public async Task<IActionResult> AddToShoppingCart(ShoppingCartCreationDto shoppingCartCreationDto,
        CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"POST /api/shopping-carts/user-cart CALLED TO ADD SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartEntity = _mapper.Map<ShoppingCart>(shoppingCartCreationDto);

        await _shoppingCartsRepository.AddShoppingCartAsync(userId, cartEntity, cancellationToken);
        await _pixelMartRepository.SaveAsync(cancellationToken);

        var cartToReturn = _mapper.Map<ShoppingCartDto>(cartEntity);

        return CreatedAtRoute("GetCartForUser", cartToReturn);
    }

    [HttpPut("user-cart")]
    public async Task<IActionResult> UpdateShoppingCart(ShoppingCartManipulationDto shoppingCartUpdateDto,
        CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"PUT /api/shopping-carts/user-cart CALLED TO UPDTAE THE SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartFromRepo = await _shoppingCartsRepository.GetCartDetailsForUserAsync(userId, cancellationToken);

        if (cartFromRepo == null)
        {
            var CartToAdd = _mapper.Map<ShoppingCart>(shoppingCartUpdateDto);

            await _shoppingCartsRepository.AddShoppingCartAsync(userId, CartToAdd, cancellationToken);
            await _pixelMartRepository.SaveAsync(cancellationToken);

            return CreatedAtRoute("GetCartForUser", CartToAdd);
        }

        _mapper.Map(shoppingCartUpdateDto, cartFromRepo);
        await _shoppingCartsRepository.UpdateShoppingCartAsync(userId, cartFromRepo, cancellationToken);

        return NoContent();
    }

    [HttpPatch("user-cart")]
    public async Task<IActionResult> PartiallyUpdateShoppingCart(JsonPatchDocument<ShoppingCartUpdateDto> patchDocument,
        CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"PATCH /api/shopping-carts/user-cart CALLED TO PARTIALLY UPDTAE THE SHOPPING CART FOR A USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartFromRepo = await _shoppingCartsRepository.GetCartDetailsForUserAsync(userId, cancellationToken);

        if (cartFromRepo == null)
        {
            var shoppingCartDto = new ShoppingCartUpdateDto();

            patchDocument.ApplyTo(shoppingCartDto, ModelState);

            if (!TryValidateModel(shoppingCartDto))
                return ValidationProblem(ModelState);

            var cartToAdd = _mapper.Map<ShoppingCart>(shoppingCartDto);

            await _shoppingCartsRepository.AddShoppingCartAsync(userId, cartToAdd, cancellationToken);
            await _pixelMartRepository.SaveAsync(cancellationToken);

            var cartToReturn = _mapper.Map<ShoppingCartDto>(cartToAdd);

            return CreatedAtRoute("GetCartForUser", cartToReturn);
        }

        var shoppingCartToPatch = _mapper.Map<ShoppingCartUpdateDto>(cartFromRepo);

        patchDocument.ApplyTo(shoppingCartToPatch, ModelState);

        if (!TryValidateModel(shoppingCartToPatch))
            return ValidationProblem(ModelState);

        _mapper.Map(shoppingCartToPatch, cartFromRepo);
        await _shoppingCartsRepository.UpdateShoppingCartAsync(userId, cartFromRepo, cancellationToken);

        return NoContent();
    }

    [HttpDelete("user-cart")]
    public async Task<IActionResult> DeleteUserShoppingCart(CancellationToken cancellationToken)
    {
        _requestLogHelper.LogInfo($"DELETE /api/shopping-carts/user-cart CALLED TO DELETE SHOPPING CART FOR USER");

        var userId = getCurrentUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var cartForUserFromRepo = await _shoppingCartsRepository.GetCartDetailsForUserAsync(userId, cancellationToken);

        if (cartForUserFromRepo is null)
            return NotFound();

        await _shoppingCartsRepository.DeleteShoppingCartAsync(cartForUserFromRepo, cancellationToken);

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
