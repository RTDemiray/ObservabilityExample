using Common.Shared.DTOs;

namespace Order.API.Models;

public record OrderCreateRequestDto(int UserId, List<OrderItemDto> Items);