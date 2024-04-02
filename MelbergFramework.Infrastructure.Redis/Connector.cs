using StackExchange.Redis;

namespace MelbergFramework.Infrastructure.Redis;

public interface IConnector
{
    IDatabaseAsync GetDatabaseAsync(string connectionString);
    void ConnectToDatabase(string connectionString);
}

public class Connector : IConnector
{
    private Dictionary<string,ConnectionMultiplexer> Connections;
    public Connector() 
    {
        Connections = new();
    }

    public void ConnectToDatabase(string connectionString)
    {
        if(!Connections.ContainsKey(connectionString))
        {
            Connections.Add(connectionString,ConnectionMultiplexer.Connect(connectionString));
        }
    }

    public IDatabaseAsync GetDatabaseAsync(string connectionString) =>
        Connections[connectionString].GetDatabase();
    
}
