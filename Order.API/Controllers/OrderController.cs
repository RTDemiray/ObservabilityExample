using Common.Shared.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Services;

namespace Order.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly IBus _bus;

    public OrderController(OrderService orderService, IBus bus)
    {
        _orderService = orderService;
        _bus = bus;
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateRequestDto requestDto)
    {
        var result = await _orderService.CreateAsync(requestDto);
        
        return new ObjectResult(result) { StatusCode = result.StatusCode };
    }

    [HttpGet]
    public async Task<IActionResult> SendOrderCreatedEvent()
    {
        await _bus.Publish(new OrderCreatedEvent(Guid.NewGuid().ToString()));
        
        return Ok();
    }
}