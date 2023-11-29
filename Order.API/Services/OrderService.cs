using System.Diagnostics;
using Common.Shared.DTOs;
using MassTransit;
using OpenTelemetry.Shared;
using Order.API.Models;

namespace Order.API.Services;

public class OrderService
{
    private readonly OrderDbContext _context;
    private readonly StockService _stockService;
    private readonly RedisService _redisService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(OrderDbContext context, StockService stockService, RedisService redisService, IBus bus, ILogger<OrderService> logger)
    {
        _context = context;
        _stockService = stockService;
        _redisService = redisService;
        _logger = logger;
    }

    public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto request)
    {
        await _redisService.GetDb(0).StringSetAsync("UserId", request.UserId);
        
        Activity.Current?.SetTag(".NET Span", "span value");
        using var activity = ActivitySourceProvider.Source.StartActivity();
        activity?.AddEvent(new ActivityEvent("Sipariş süreci başladı."));
        activity?.SetBaggage("UserId", request.UserId.ToString());
        
        var order = new Models.Order
        {
            Created = DateTime.UtcNow,
            OrderCode = Guid.NewGuid().ToString(),
            Status = OrderStatus.Success,
            UserId = request.UserId,
            Items = request.Items.Select(x => new OrderItem
            {
                Count = x.Count,
                ProductId = x.ProductId,
                UnitPrice = x.UnitPrice
            }).ToList()
        };
        
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Sipariş veritabanına kaydedildi.{@userId}", request.UserId);

        var stockRequest = new StockCheckAndPaymentProccessRequestDto(order.OrderCode, request.Items);

        var (isSuccess, failMessage) = await _stockService.CheckStockAndPaymentStartAsync(stockRequest);

        if (!isSuccess)
            return ResponseDto<OrderCreateResponseDto>.Fail(StatusCodes.Status500InternalServerError, failMessage!);
        
        activity?.AddEvent(new ActivityEvent("Sipariş süreci tamamlandı"));

        return ResponseDto<OrderCreateResponseDto>.Success(StatusCodes.Status200OK,
            new OrderCreateResponseDto(order.Id));
    }
}