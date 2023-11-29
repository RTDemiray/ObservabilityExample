using Common.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentProccessController : ControllerBase
{
    private readonly ILogger<PaymentProccessController> _logger;

    public PaymentProccessController(ILogger<PaymentProccessController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Create(PaymentCreateRequestDto request)
    {
        const decimal balance = 1000;

        if (request.TotalPrice > balance)
        {
            _logger.LogWarning("Yetersiz bakiye! orderCode:{@orderCode}", request.OrderCode);
            return BadRequest(
                ResponseDto<PaymentCreateResponseDto>.Fail(StatusCodes.Status400BadRequest, "Yetersiz bakiye!"));

        }
            
        _logger.LogInformation("Kart işlemi başarıyla gerçekleşmiştir. orderCode:{@orderCode}", request.OrderCode);
        
        return Ok(ResponseDto<PaymentCreateResponseDto>.Success(StatusCodes.Status200OK,
            new PaymentCreateResponseDto("Kart işlemi başarıyla gerçekleşmiştir.")));
    }
}