using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockRedisValue
{
    public List<RedisValue> Values { get; set; }
    public bool IsSet { get; set; }
    public bool IsLocked { get; set; }
    public string Lease { get; set; } = string.Empty;
    public MockRedisValue(RedisValue value) 
    { 
        Values = new(){value}; 
    }
}
