using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/shopping-carts")]
public class ShoppingCartsController : ControllerBase
{
    public ShoppingCartsController()
    {

    }
}
