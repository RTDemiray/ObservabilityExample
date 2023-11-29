using Common.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Stock.API.Services;

namespace Stock.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpPost]
    public async Task<IActionResult> CheckAndPaymentStart(StockCheckAndPaymentProccessRequestDto request)
    {
        var result = await _stockService.CheckAndPaymentProccessAsync(request);

        return new ObjectResult(result) { StatusCode = result.StatusCode };
    }
}