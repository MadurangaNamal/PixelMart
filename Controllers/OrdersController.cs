using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Entities;
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
        var ordersResponse = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return Ok(ordersResponse);
    }

    [HttpGet("user-order")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId()
    {
        _requestLogHelper.LogInfo($"GET /api/orders/user-order CALLED TO RETRIEVE ORDERS FOR A USER");

        var userId = _requestLogHelper.GetUserId();
        var orders = (userId != Guid.Empty) ? await _pixelMartRepository.GetOrdersForUserAsync(userId) : null!;

        if (orders == null || !orders.Any())
            return NotFound($"No orders found for user");

        var ordersResponse = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return Ok(ordersResponse);
    }

    [HttpGet("user-order/{orderId}", Name = "GetOrderById")]
    public async Task<ActionResult<OrderDto>> GetOrderByOrderId(Guid orderId)
    {
        _requestLogHelper.LogInfo($"GET /api/orders/user-order/orderId CALLED TO RETRIEVE AN ORDER FOR A USER");

        var userId = _requestLogHelper.GetUserId();
        var order = (userId != Guid.Empty) ? await _pixelMartRepository.GetOrderForUserAsync(userId, orderId) : null!;

        if (order is null)
            return NotFound($"Order not found for user");

        var orderResponse = _mapper.Map<OrderDto>(order);

        return Ok(orderResponse);
    }

    [HttpPost("user-order")]
    public async Task<IActionResult> CreateNewOrder(OrderCreationDto orderCreationDto)
    {
        _requestLogHelper.LogInfo($"POST /api/orders/user-order CALLED TO CREATE AN ORDER FOR A USER");

        var userId = _requestLogHelper.GetUserId();

        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var orderEntity = _mapper.Map<Order>(orderCreationDto);

        await _pixelMartRepository.CreateOrderAsync(userId, orderEntity);
        await _pixelMartRepository.SaveAsync();

        var orderToReturn = _mapper.Map<OrderDto>(orderEntity);

        return CreatedAtRoute("GetOrderById", new { orderId = orderToReturn.Id }, orderToReturn);
    }

    [HttpPut("user-order/{orderId}")]
    public async Task<IActionResult> UpdateOrder(Guid orderId, OrderUpdateDto orderUpdateDto)
    {
        _requestLogHelper.LogInfo($"PUT /api/orders/user-order/{orderId} CALLED TO UPDATE AN ORDER FOR A USER");

        var userId = _requestLogHelper.GetUserId();

        if (orderUpdateDto == null)
            return BadRequest("Order update data is required.");

        var orderFromRepo = await _pixelMartRepository.GetOrderForUserAsync(userId, orderId);

        if (orderFromRepo is null)
            return NotFound($"Order with ID {orderId} not found for the user.");

        _mapper.Map(orderUpdateDto, orderFromRepo);
        await _pixelMartRepository.UpdateOrderAsync(userId, orderId, orderFromRepo);

        return NoContent();
    }

    [HttpDelete("user-order/{orderId}")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        _requestLogHelper.LogInfo($"DELETE /api/orders/user-order/{orderId} CALLED TO CANCEL AN ORDER FOR A USER");

        var userId = _requestLogHelper.GetUserId();
        
        if (userId == Guid.Empty)
            return BadRequest("Invalid user ID");

        var orderFromRepo = await _pixelMartRepository.GetOrderForUserAsync(userId, orderId);
        
        if (orderFromRepo is null)
            return NotFound($"Order with ID {orderId} not found for the user.");

        await _pixelMartRepository.CancelOrderAsync(orderFromRepo);
        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }
}
