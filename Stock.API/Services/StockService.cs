using Common.Shared.DTOs;

namespace Stock.API.Services;

public class StockService
{
    private readonly PaymentService _paymentService;
    private readonly ILogger<StockService> _logger;

    public StockService(PaymentService paymentService, ILogger<StockService> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    private static Dictionary<int, int> GetProductStockList()
    {
        Dictionary<int, int> productStockList = new() { { 1, 10 }, { 2, 20 }, { 3, 30 } };
        
        return productStockList;
    }

    public async Task<ResponseDto<StockCheckAndPaymentProccessResponseDto>> CheckAndPaymentProccessAsync(
        StockCheckAndPaymentProccessRequestDto request)
    {
        var productStockList = GetProductStockList();
        var stockStatus = new List<(int productId, bool hasStockExist)>();

        foreach (var orderItem in request.OrderItems)
        {
            var hasExistStock = productStockList.Any(x => x.Key == orderItem.ProductId && x.Value >= orderItem.Count);

            stockStatus.Add((orderItem.ProductId, hasExistStock));
        }

        if (stockStatus.Any(x => x.hasStockExist == false))
        {
            return ResponseDto<StockCheckAndPaymentProccessResponseDto>.Fail(
                StatusCodes.Status400BadRequest,
                "Stok yetersiz!");
        }

        _logger.LogInformation("Stock ayrıldı. orderCode:{@orderCode}", request.OrderCode);

        var (isSuccess, failMessage) =
            await _paymentService.CreatePaymentProccessAsync(new PaymentCreateRequestDto(request.OrderCode,
                request.OrderItems.Sum(o => o.UnitPrice)));

        if (!isSuccess)
            return ResponseDto<StockCheckAndPaymentProccessResponseDto>.Fail(StatusCodes.Status400BadRequest,
                failMessage!);
        
        return ResponseDto<StockCheckAndPaymentProccessResponseDto>.Success(StatusCodes.Status200OK,
            new StockCheckAndPaymentProccessResponseDto("Ödeme süreci tamamlandı."));
    }
}