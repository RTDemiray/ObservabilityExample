using System.Diagnostics;
using System.Text.Json;
using Common.Shared.Events;
using MassTransit;

namespace Stock.API.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Activity.Current?.SetTag("message.body", JsonSerializer.Serialize(context.Message));
        
        return Task.CompletedTask;
    }
}