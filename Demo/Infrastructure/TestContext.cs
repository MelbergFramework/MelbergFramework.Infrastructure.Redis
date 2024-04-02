using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace Demo.Infrastructure;

public class TestContext : RedisContext
{
    public TestContext(IOptions<RedisConnectionOptions<TestContext>> options,
            IConnector connector) :
        base(options.Value, connector)
    {
    }
}
