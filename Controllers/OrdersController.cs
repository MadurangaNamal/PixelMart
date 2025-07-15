using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Identity;
using PixelMart.API.Models.Order;
using PixelMart.API.Repositories;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public OrdersController(IPixelMartRepository pixelMartRepository, IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        _requestLogHelper.LogInfo("GET /api/orders CALLED TO RETRIEVE ALL ORDERS");
        var orders = await _pixelMartRepository.GetAllOrdersAsync();

        return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("user-order")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId()
    {
        _requestLogHelper.LogInfo($"GET /api/orders/user-order CALLED TO RETRIEVE ORDERS FOR A USER");

        var userId = _requestLogHelper.GetUserID();
        var orders = (userId != Guid.Empty) ? await _pixelMartRepository.GetOrdersForUserAsync(userId) : null!;

        if (orders == null || !orders.Any())
            return NotFound($"No orders found for user");

        return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderByOrderId(Guid orderId)
    {
        _requestLogHelper.LogInfo($"GET /api/orders/order/orderId CALLED TO RETRIEVE AN ORDER FOR A USER");

        var userId = _requestLogHelper.GetUserID();
        var order = (userId != Guid.Empty) ? await _pixelMartRepository.GetOrderForUserAsync(userId, orderId) : null!;

        if (order is null)
            return NotFound($"Order not found for user");

        return Ok(_mapper.Map<OrderDto>(order));
    }

}
