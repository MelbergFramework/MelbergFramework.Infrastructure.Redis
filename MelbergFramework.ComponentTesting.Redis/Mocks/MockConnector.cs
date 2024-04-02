using MelbergFramework.Infrastructure.Redis;
using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockConnector : IConnector
{
    private Dictionary<string,MockDatabaseAsync> _connections;

    public MockConnector() { _connections = new(); }

    public void ConnectToDatabase(string connectionString) { }


    public IDatabaseAsync GetDatabaseAsync(string connectionString)
    {
        if(!_connections.ContainsKey(connectionString))
        {
            _connections.Add(connectionString,new MockDatabaseAsync());
        }

        return _connections[connectionString];
    }
}
