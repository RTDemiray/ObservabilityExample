namespace Common.Shared.DTOs;

public record StockCheckAndPaymentProccessRequestDto(string OrderCode, List<OrderItemDto> OrderItems);