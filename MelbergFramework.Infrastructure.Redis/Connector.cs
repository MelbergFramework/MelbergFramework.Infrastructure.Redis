using StackExchange.Redis;

namespace MelbergFramework.Infrastructure.Redis;

public interface IConnector
{
    ConnectionMultiplexer GetDatabaseAsync(string connectionString);
}
public class Connector : IConnector
{
    public Connector() { }

    public ConnectionMultiplexer GetDatabaseAsync(string connectionString) =>
        ConnectionMultiplexer.Connect(connectionString);
}
