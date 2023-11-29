namespace Common.Shared.DTOs;

public record PaymentCreateRequestDto(string OrderCode, decimal TotalPrice);