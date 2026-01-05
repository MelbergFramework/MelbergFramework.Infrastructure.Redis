using MelbergFramework.Infrastructure.Redis;
using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockConnector(RedisFixture fixture) : IConnector
{
    public void ConnectToDatabase(string connectionString) { }

    public IDatabaseAsync GetDatabaseAsync(string connectionString) => fixture.Connection.GetDatabase();
}
