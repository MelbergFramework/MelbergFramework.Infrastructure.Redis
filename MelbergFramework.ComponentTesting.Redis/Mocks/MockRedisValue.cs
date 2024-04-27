using StackExchange.Redis;

namespace MelbergFramework.ComponentTesting.Redis.Mocks;

public class MockRedisValue
{
    public List<RedisValue> Values { get; set; }
    public MockRedisType Type { get; set; }
    public bool IsLocked { get; set; }
    public DateTime TimeOfDeath {get; set;} = DateTime.MaxValue;
    public string Lease { get; set; } = string.Empty;
    public MockRedisValue(RedisValue value, MockRedisType type) 
    { 
        Values = new(){value}; 
        Type = type;
    }
}
