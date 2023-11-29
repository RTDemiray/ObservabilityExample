using Common.Shared.Middlewares;
using Log.Shared;
using OpenTelemetry.Shared;
using Stock.API.Services;
using MassTransit;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Stock.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry(builder.Configuration);
builder.Services.AddScoped<StockService>();
builder.Services.AddHttpClient<PaymentService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices")["PaymentApi"]!);
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("stock.order-created-event.queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
    });
});
builder.Host.UseSerilog(Logging.ConfigureLogging());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
app.UseMiddleware<RequestAndResponseActivityMiddleware>();
app.UseExceptionMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();