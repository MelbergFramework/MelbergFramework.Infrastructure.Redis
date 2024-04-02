using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace MelbergFramework.Infrastructure.Redis;

public class RedisContext 
{
    private IConnector _connection;
    private string _uri;
    public RedisContext(
            IRedisConnectionOptions options,
            IConnector connector)
    {
        connector.ConnectToDatabase(options.Uri);

        _connection = connector;
        _uri = options.Uri;
    }

    public IDatabaseAsync DB => _connection.GetDatabaseAsync(_uri);
}
