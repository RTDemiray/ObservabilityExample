using Common.Shared.Middlewares;
using Log.Shared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry(builder.Configuration);
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisService = sp.GetRequiredService<RedisService>();
    return redisService.GetConnectionMultiplexer;
});
builder.Services.AddSingleton<RedisService>(_ => new RedisService(builder.Configuration));
builder.Services.AddHttpClient<StockService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices")["StockApi"]!);
});
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddMassTransit(x => x.UsingRabbitMq());
builder.Host.UseSerilog(Logging.ConfigureLogging());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var appDbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    appDbContext.Database.Migrate();
}

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