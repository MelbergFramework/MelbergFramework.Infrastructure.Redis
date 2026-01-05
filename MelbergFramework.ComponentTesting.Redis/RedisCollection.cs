using Xunit;

namespace MelbergFramework.ComponentTesting.Redis;

[CollectionDefinition("Redis Collection")]
public class RedisCollection : ICollectionFixture<RedisFixture>
{
    // This class is intentionally left empty.
    // Its purpose is to associate the RedisFixture with the "Redis Collection".
}
