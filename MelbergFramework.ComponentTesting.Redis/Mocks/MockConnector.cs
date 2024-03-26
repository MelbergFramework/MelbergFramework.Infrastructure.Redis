using MelbergFramework.Infrastructure.Redis;
using Moq;
using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockConnector : IConnector
{
    private Dictionary<string,MockDatabaseAsync> _connections;

    public MockConnector() { _connections = new(); }

    public ConnectionMultiplexer GetDatabaseAsync(string connectionString)
    {
        if(!_connections.ContainsKey(connectionString))
        {
            _connections.Add(connectionString,new MockDatabaseAsync());
        }

        var mockMulti = new Mock<ConnectionMultiplexer>();

        mockMulti.Setup(_ => _.GetDatabase(-1,null))
            .Returns((IDatabase)_connections[connectionString]);

        return mockMulti.Object;
    }
}
