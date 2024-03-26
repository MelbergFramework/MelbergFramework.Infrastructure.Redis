using Microsoft.Extensions.Options;
using StackExchange.Redis;


namespace MelbergFramework.Infrastructure.Redis;
public class RedisContext 
{
    private ConnectionMultiplexer _connection;
    public RedisContext(
            IOptions<RedisConnectionOptions<RedisContext>> options,
            IConnector connector)
    {
        _connection = connector.GetDatabaseAsync(options.Value.Uri);
    }

    public IDatabaseAsync DB => _connection.GetDatabase();
}
