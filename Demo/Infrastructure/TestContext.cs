using MelbergFramework.Infrastructure.Redis;

namespace Demo.Infrastructure;

public class TestContext : RedisContext
{
    public TestContext(IConfiguration provider) : base(provider) { }
}