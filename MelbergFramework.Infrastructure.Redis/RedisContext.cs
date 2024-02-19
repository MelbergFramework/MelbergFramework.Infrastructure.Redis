using Microsoft.Extensions.Configuration;
using StackExchange.Redis;


namespace MelbergFramework.Infrastructure.Redis;
public class RedisContext
{
    private ConnectionMultiplexer _connection;
    public RedisContext(IConfiguration provider)
    {
        string connectionString = provider.GetConnectionString(this.GetType().Name);
        if(string.IsNullOrEmpty(connectionString))
        {
            throw new Exception($"Connection string for {this.GetType().Name} is missing");
        }
        _connection = ConnectionMultiplexer.Connect(connectionString);
        
    }

    public IDatabaseAsync DB => _connection.GetDatabase();
}