using StackExchange.Redis;

namespace Order.API.Services;

public class RedisService
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;

    public RedisService(IConfiguration configuration)
    {
        var host = configuration.GetSection("Redis")["Host"]!;
        var port = configuration.GetSection("Redis")["Port"]!;
        var config = $"{host}:{port}";

        _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
    }

    public ConnectionMultiplexer GetConnectionMultiplexer => _connectionMultiplexer;

    public IDatabase GetDb(int db) => _connectionMultiplexer.GetDatabase(db);
}