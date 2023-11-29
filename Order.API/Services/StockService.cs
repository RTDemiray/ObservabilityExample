using Common.Shared.DTOs;

namespace Order.API.Services;

public class StockService
{
    private readonly HttpClient _client;

    public StockService(HttpClient client)
    {
        _client = client;
    }

    public async Task<(bool isSuccess, string? failMessage)> CheckStockAndPaymentStartAsync(
        StockCheckAndPaymentProccessRequestDto request)
    {
        var response = await _client.PostAsJsonAsync("api/Stock/CheckAndPaymentStart", request);

        var responseContent = await response.Content.ReadFromJsonAsync<ResponseDto<StockCheckAndPaymentProccessResponseDto>>();

        return response.IsSuccessStatusCode ? (true, null) : (false, responseContent!.Errors!.First());
    }
}